using System.Data;
using System.Data.SQLite;

namespace MonitorWang.Core.Database.SQLite
{
    /// <summary>
    /// Provides a SQLite specific sql statement builder
    /// </summary>
    public class SQLiteStatement : SqlStatementBase<SQLiteStatement>
    {
        public static SQLiteStatement Create(string statement, params object[] args)
        {
            var sql = new SQLiteStatement();
            sql.myCmdBuilder.AppendFormat(statement, args);
            return sql;
        }

        public override IDataParameter NewParameter(string name, object value)
        {
            return new SQLiteParameter(name, value);
        }
    }
}