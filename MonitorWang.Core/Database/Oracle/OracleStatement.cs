using System.Data;
using System.Data.OracleClient;

namespace MonitorWang.Core.Database.Oracle
{
    /// <summary>
    /// Provides a SqlServer specific sql statement builder
    /// </summary>
    public class OracleStatement : SqlStatementBase<OracleStatement>
    {
        public static OracleStatement Create(string statement, params object[] args)
        {
            var sql = new OracleStatement();
            sql.myCmdBuilder.AppendFormat(statement, args);
            return sql;
        }

        public OracleStatement Done()
        {
            return this;
        }

        public override IDataParameter NewParameter(string name, object value)
        {
            return new OracleParameter(name, value);
        }
    }
}