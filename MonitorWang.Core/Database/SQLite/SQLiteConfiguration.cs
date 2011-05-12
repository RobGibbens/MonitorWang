using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Database.SQLite
{
    public class SQLiteConfiguration : PluginConfigBase
    {
        public string ConnectionString { get; set; }
    }
}