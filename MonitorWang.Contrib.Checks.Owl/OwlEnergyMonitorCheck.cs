using System;
using MonitorWang.Core;
using MonitorWang.Core.Database.SQLite;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Contrib.Checks.Owl
{
    public class OwlEnergyMonitorCheckConfig : PluginConfigBase
    {
        public string ConnectionString { get; set; }
    }

    public class OwlEnergyMonitorCheck : IHealthCheckPlugin
    {
        protected readonly OwlEnergyMonitorCheckConfig myConfig;
        protected PluginDescriptor myIdentity;
        protected DateTime myLastRun;

        public OwlEnergyMonitorCheck(OwlEnergyMonitorCheckConfig config)
        {
            myConfig = config;

            myIdentity = new PluginDescriptor
            {
                Description = string.Format("Reports energy information from your Owl Monitor Database"),
                TypeId = new Guid("55728986-4638-4b9c-8C8A-FF9F4290F564"),
                Name = myConfig.FriendlyId
            };

            myLastRun = DateTime.Now;
        }

        public void Initialise()
        {
            //myLastRun = new DateTime(2010, 11, 23,23,30,00);
        }

        public Status Status { get; set; }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public void Execute()
        {
            var lastRun = Convert.ToInt64(myLastRun.ToString("yyyMMddHHmm"));

            using (var query = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create("select s.name, datetime(year || '-' || substr('0' || month, -2,2) || '-' || substr('0' || day, -2,2) || ' ' || substr('0' || hour, -2,2) || ':' || substr('0' || min, -2,2)) [recorded], ")
                .Append("ch1_amps_min, ch1_amps_avg, ch1_amps_max, ch1_kw_min, CAST(ch1_kw_avg AS REAL) [ch1_kw_avg], ch1_kw_max")
                .Append("from energy_history h join energy_sensor s on h.addr = s.addr")
                .Append("where CAST((year || substr('0' || month, -2,2) || substr('0' || day, -2,2) || substr('0' || hour, -2,2) || substr('0' || min, -2,2)) AS INTEGER) > ")
                .InsertParameter("@lastrun", lastRun)))
            {
                using (var reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // assumes that results are in ascending order
                        myLastRun = Convert.ToDateTime(reader["recorded"]);

                        var sensor = reader["name"].ToString();
                        var data = HealthCheckData.For(Identity, sensor)
                            .AddTag(sensor);

                        // report average watts
                        data.ResultCount = Convert.ToDouble(reader["ch1_kw_avg"]);
                        data.GeneratedOnUtc = myLastRun.ToUniversalTime();
                        data.Properties = new ResultProperties();

                        Messenger.Publish(data);
                        Logger.Debug("[{0}] Sent reading {1}kw taken {2}", 
                            Identity.Name,
                            data.ResultCount,
                            data.GeneratedOnUtc.ToLocalTime());
                    }
                }
            }
        }
    }
}
