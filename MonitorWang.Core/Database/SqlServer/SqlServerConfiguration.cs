using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Database.SqlServer
{
    public class SqlServerConfiguration : PluginConfigBase
    {
        public string ConnectionString { get; set; }
    }
}