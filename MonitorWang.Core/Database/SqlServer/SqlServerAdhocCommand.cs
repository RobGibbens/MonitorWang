using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MonitorWang.Core.Database.SqlServer
{
    /// <summary>
    /// Fluent wrapper to core data access components to execute an ad-hoc (inline) sql statement
    /// </summary>
    public class SqlServerAdhocCommand : AdhocCommandBase
    {
        /// <summary>
        /// This attempts 
        /// </summary>
        /// <returns></returns>
        public static SqlServerAdhocCommand UsingSmartConnection(string connection)
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
        public static SqlServerAdhocCommand UsingConnectionNamed(string connectionName)
        {
            return UsingConnectionString(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);
        }

        /// <summary>
        /// Main entry point to fluent interface. Creates a new instance of 
        /// an Adhoc database operation with the connection string specified
        /// </summary>
        /// <returns></returns>
        public static SqlServerAdhocCommand UsingConnectionString(string connectionString)
        {
            return new SqlServerAdhocCommand
                       {
                           myConnectionString = connectionString
                       };
        }

        protected override IDbConnection NewConnection()
        {
            return new SqlConnection();
        }
    }
}