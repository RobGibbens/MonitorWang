using System;
using System.Diagnostics;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Database.SqlServer;

namespace MonitorWang.Core.Checks
{
    public class ScalarCheckConfigBase : PluginConfigBase
    {
        /// <summary>
        /// The portion of the query that starts at the FROM statement. This will 
        /// be prefixed with SELECT COUNT(*) to ensure the query returns a scalar
        /// result.
        /// </summary>
        public string FromQuery { get; set; }

        /// <summary>
        /// By default, zero rows returned from a query would be interpretted
        /// as a success (eg: you are looking for critial applications errors
        /// in the event log). If you are expecting rows to be returned then
        /// set this to true should no rows be returned.
        /// </summary>
        public bool InterpretZeroRowsAsAFailure { get; set; }

        /// <summary>
        /// Depending upon how the rowcount is interpreted via
        /// <see cref="InterpretZeroRowsAsAFailure"/> the check will only
        /// publish a result if its a failure
        /// </summary>
        public bool PublishOnlyIfFailure { get; set; }
    }
    public abstract class ScalarCheckBase : IHealthCheckPlugin
    {
        protected readonly ScalarCheckConfigBase myBaseConfig;
        protected PluginDescriptor  myIdentity;

        /// <summary>
        /// default ctor
        /// </summary>
        protected ScalarCheckBase(ScalarCheckConfigBase config)
        {
            myBaseConfig = config;
        }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public Status Status { get; set; }

        public void Initialise()
        {
            // do nothing
        }

        public void Execute()
        {
            if (!myBaseConfig.FromQuery.StartsWith("FROM ", StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException("The FromQuery config property is badly formed; it must start with 'FROM'");
            ValidateConfig();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Create the query
            var query = SqlServerStatement.Create("SELECT COUNT(*) ")
                .Append(myBaseConfig.FromQuery).ToString();

            // Execute the query
            var rowcount = RunQuery(query);

            stopwatch.Stop();

            // is this a good or bad result?
            var result = DecideResult(myBaseConfig, rowcount);
            // bail if success and only interested in failures
            if (result && myBaseConfig.PublishOnlyIfFailure)
                // result = success, but only interested in failures
                return;

            OnPublish(result, rowcount, stopwatch.Elapsed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract int RunQuery(string query);

        /// <summary>
        /// Allows a subclass to validate its config properties
        /// </summary>
        protected virtual void ValidateConfig()
        {
            // do nothing
        }

        /// <summary>
        /// Implements logic to decide whether this query produces a successful or not result
        /// </summary>
        /// <param name="config"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        protected virtual bool DecideResult(ScalarCheckConfigBase config, int rowcount)
        {
            bool result;

            if (myBaseConfig.InterpretZeroRowsAsAFailure)
            {
                result = (rowcount > 0);
            }
            else
            {
                result = (rowcount == 0);
            }

            return result;
        }

        /// <summary>
        /// Override this if you want to alter the publishing behaviour
        /// </summary>
        /// <param name="result"></param>
        /// <param name="rowcount"></param>
        /// <param name="duration"></param>
        protected virtual void OnPublish(bool result, int rowcount, TimeSpan duration)
        {
            Messenger.Publish(new HealthCheckData
            {
                Identity = Identity,
                Result = result,
                ResultCount = rowcount,
                Info = string.Format("{0} rows returned", rowcount),
                Properties = new ResultProperties
                                 {
                                     { "Rowcount", rowcount.ToString() },
                                     { "Duration(ms)", duration.TotalMilliseconds.ToString() },
                                     { "Criteria", myBaseConfig.FromQuery }
                                 }
            });
        }
    }
}