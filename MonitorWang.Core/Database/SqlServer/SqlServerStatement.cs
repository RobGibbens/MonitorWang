using System.Data;
using System.Data.SqlClient;

namespace MonitorWang.Core.Database.SqlServer
{
    /// <summary>
    /// Provides a SqlServer specific sql statement builder
    /// </summary>
    public class SqlServerStatement : SqlStatementBase<SqlServerStatement>
    {
        public static SqlServerStatement Create(string statement, params object[] args)
        {
            var sql = new SqlServerStatement();
            sql.myCmdBuilder.AppendFormat(statement, args);
            return sql;
        }

        public SqlServerStatement Done()
        {
            return this;
        }

        public override IDataParameter NewParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }
    }
}