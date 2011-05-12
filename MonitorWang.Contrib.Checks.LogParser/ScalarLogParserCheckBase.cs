using System;
using MonitorWang.Core.Checks;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public abstract class ScalarLogParserCheckBase : ScalarCheckBase
    {
        protected object myInputContext;

        protected ScalarLogParserCheckBase(ScalarCheckConfigBase config) 
            : base(config)
        {
        }

        protected override int RunQuery(string query)
        {
            // Instantiate the LogQuery object
            var logQuery = new LogQueryClassClass();

            // cache the input context, only ask for it once
            if (myInputContext == null)
                myInputContext = GetInputContext();

            // Execute the query
            var rowcount = 0;
            var oRecordSet = logQuery.Execute(query, myInputContext);

            // Browse the recordset
            for (; !oRecordSet.atEnd(); oRecordSet.moveNext())
                rowcount = Convert.ToInt32(oRecordSet.getRecord().toNativeString(""));
            // Close the recordset
            oRecordSet.close();

            return rowcount;
        }

        /// <summary>
        /// Returns the correct Input Context class for this query. It will also sanity check
        /// all relevant config params and throw exceptions where there are violations (required
        /// setting missing, badly formed etc)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if a setting is missing</exception>
        /// <exception cref="FormatException">Thrown if a setting is badly formed</exception>
        protected abstract object GetInputContext();
    }
}