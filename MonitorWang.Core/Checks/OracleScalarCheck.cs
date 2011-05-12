using System;
using MonitorWang.Core.Database.Oracle;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Checks
{
    public class OracleScalarCheckConfig : ScalarCheckConfigBase
    {
        public string ConnectionString { get; set; }
    }

    public class OracleScalarCheck : ScalarCheckBase
    {
        protected OracleScalarCheckConfig myConfig;

        /// <summary>
        /// default ctor
        /// </summary>
        public OracleScalarCheck(OracleScalarCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
            {
                Description = "Oracle Scalar Check",
                TypeId = new Guid("FF47074E-798B-4e29-B098-D306EC5B5666"),
                Name = myBaseConfig.FriendlyId
            };
        }

        protected override void ValidateConfig()
        {
            base.ValidateConfig();

            if (myConfig.FromQuery.Contains(";"))
                throw new FormatException("Semi-colons are not accepted in Sql from-query statements");
        }

        protected override int RunQuery(string query)
        {
            int rowcount;

            using (var cmd = OracleAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(query))
            {
                rowcount = (int)cmd.ExecuteScalar();
            }

            return rowcount;
        }
    }
}