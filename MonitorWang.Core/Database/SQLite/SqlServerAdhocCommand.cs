using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace MonitorWang.Core.Database.SQLite
{
    /// <summary>
    /// Fluent wrapper to core data access components to execute an ad-hoc (inline) sql statement
    /// </summary>
    public class SQLiteAdhocCommand : AdhocCommandBase
    {
        /// <summary>
        /// This attempts 
        /// </summary>
        /// <returns></returns>
        public static SQLiteAdhocCommand UsingSmartConnection(string connection)
        {
            return (ConfigurationManager.ConnectionStrings[connection] != null)
                       ? UsingConnectionNamed(connection)
                       : UsingConnectionString(connection);
        }

        /// <summary>
        /// Main entry point to fluent interface. Creates a new instance of 
        /// an Adhoc database operation with the connection string name specified
        /// </summary>
        /// <returns></returns>
        public static SQLiteAdhocCommand UsingConnectionNamed(string connectionName)
        {
            return UsingConnectionString(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);
        }

        /// <summary>
        /// Main entry point to fluent interface. Creates a new instance of 
        /// an Adhoc database operation with the connection string specified
        /// </summary>
        /// <returns></returns>
        public static SQLiteAdhocCommand UsingConnectionString(string connectionString)
        {
            return new SQLiteAdhocCommand
                       {
                           myConnectionString = connectionString
                       };
        }

        protected override IDbConnection NewConnection()
        {
            return new SQLiteConnection();
        }
    }
}