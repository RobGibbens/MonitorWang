using System;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Database.SqlServer;

namespace MonitorWang.Core.Checks
{
    public class SqlScalarCheckConfig : ScalarCheckConfigBase
    {
        public string ConnectionString { get; set; }
    }

    public class SqlScalarCheck : ScalarCheckBase
    {
        protected SqlScalarCheckConfig myConfig;

        /// <summary>
        /// default ctor
        /// </summary>
        public SqlScalarCheck(SqlScalarCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
            {
                Description = "Sql Scalar Check",
                TypeId = new Guid("7BFF8D1C-93EB-4f66-8719-5E6DDDED1E97"),
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

            using (var cmd = SqlServerAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(query))
            {
                rowcount = (int)cmd.ExecuteScalar();
            }

            return rowcount;
        }
    }
}