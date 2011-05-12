using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MonitorWang.Core.Database
{
    public interface ISqlStatement
    {
        Dictionary<string, IDataParameter> Parameters { get; set; }
        string ToString();
    }

    /// <summary>
    /// fluently builds a sql statement for use by any sub-class
    /// </summary>
    public abstract class SqlStatementBase<T> : ISqlStatement
        where T: class
    {
        protected readonly StringBuilder myCmdBuilder;

        /// <summary>
        /// The parameters for this statement
        /// </summary>
        public Dictionary<string, IDataParameter> Parameters { get; set; }

        protected SqlStatementBase()
        {
            myCmdBuilder = new StringBuilder();
            Parameters = new Dictionary<string, IDataParameter>();
        }

        /// <summary>
        /// This will append the text to the existing statement
        /// automatically including a space between existing statement
        /// and this text.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T Append(string statement, params object[] args)
        {
            return Append(statement, true, args);
        }

        /// <summary>
        /// This will append the text to the existing statement
        /// automatically including a space between existing statement
        /// and this text.
        /// </summary>
        /// <param name="test"></param>
        /// <param name="statement"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T AppendIf(Func<bool> test, string statement, params object[] args)
        {
            return test() ? Append(statement, true, args) : End();
        }

        /// <summary>
        /// This will append the text to the existing statement
        /// optionally including a space between existing statement
        /// and this text.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="includeSpace"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T Append(string statement, bool includeSpace, params object[] args)
        {
            if (includeSpace)
                myCmdBuilder.Append(" ");

            myCmdBuilder.AppendFormat(statement, args);
            return End();
        }

        /// <summary>
        /// This will append the text to the existing statement
        /// optionally including a space between existing statement
        /// and this text.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="includeSpace"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual T AppendIf(Func<bool> test, string statement, bool includeSpace, params object[] args)
        {
            return test() ? Append(statement, includeSpace, args) : End();
        }

        public virtual T And()
        {
            myCmdBuilder.Append(" AND ");
            return End();
        }

        public virtual T Or()
        {
            myCmdBuilder.Append(" OR ");
            return End();
        }

        public virtual T IsEqualTo()
        {
            myCmdBuilder.Append("=");
            return End();
        }

        public virtual T IsEqualTo(object value)
        {
            myCmdBuilder.Append("=");
            Value(value);
            return End();
        }

        public virtual T IsNotEqualTo()
        {
            myCmdBuilder.Append("<>");
            return End();
        }

        public virtual T Like()
        {
            myCmdBuilder.Append(" LIKE ");
            return End();
        }

        public virtual T OrderBy(string orderColumn)
        {
            return OrderBy(orderColumn, false);
        }

        public virtual T OrderBy(string orderColumn, bool ascending)
        {
            myCmdBuilder.AppendFormat(" order by {0} {1}", orderColumn, ascending ? "asc" : "desc");
            return End();
        }

        public virtual T ThenBy(string orderColumn)
        {
            return ThenBy(orderColumn, false);
        }

        public virtual T ThenBy(string orderColumn, bool ascending)
        {
            myCmdBuilder.AppendFormat(",{0} {1}", orderColumn, ascending ? "asc" : "desc");
            return End();
        }

        public abstract IDataParameter NewParameter(string name, object value);

        /// <summary>
        /// This will add the parameter associated to the corresponding 
        /// parameter name in the statement
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual T IncludeParameter(string name, object value)
        {
            IncludeParameter(NewParameter(name, value));
            return End();
        }

        /// <summary>
        /// This will insert a reference to the parameter into
        /// the sql statement and also add the parameter to the
        /// set that will be passed to the database command
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual T IncludeParameter(IDataParameter param, object value)
        {
            param.Value = NullCheck(value);
            return IncludeParameter(param);
        }

        /// <summary>
        /// This will add the parameter associated to the corresponding 
        /// parameter name in the statement
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual T IncludeParameter(IDataParameter param)
        {
            Parameters.Add(param.ParameterName, param);
            return End();
        }

        /// <summary>
        /// This will insert a reference to the parameter into
        /// the sql statement and also add the parameter to the
        /// set that will be passed to the database command
        /// </summary>
        /// <param name="param"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual T InsertParameter(IDataParameter param, object value)
        {
            param.Value = NullCheck(value);
            return InsertParameter(param);
        }

        /// <summary>
        /// This will insert a reference to the parameter into
        /// the sql statement and also add the parameter to the
        /// set that will be passed to the database command
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual T InsertParameter(string name, object value)
        {
            return InsertParameter(NewParameter(name, value));
        }

        /// <summary>
        /// This will insert a reference to the parameter into
        /// the sql statement and also add the parameter to the
        /// set that will be passed to the database command
        /// </summary>
        /// <param name="test"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual T InsertParameterIf(Func<bool> test, string name, object value)
        {
            return test() ? InsertParameter(NewParameter(name, value)) : End();
        }

        /// <summary>
        /// This will insert a set of parameters into the statement
        /// with a comma between each one.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T InsertParameter(params IDataParameter[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0) Append(",", false);
                Append(parameters[i].ParameterName);
                Parameters.Add(parameters[i].ParameterName, parameters[i]);
            }

            return End();
        }

        public virtual T Value(object paramValue)
        {
            if (paramValue is string)
                myCmdBuilder.AppendFormat("'{0}'", paramValue);
            else
                myCmdBuilder.AppendFormat("{0}", paramValue);
            return End();
        }

        public virtual T WildcardValue(string paramValue)
        {
            myCmdBuilder.Append(CreateWildCardValue(paramValue));
            return End();
        }

        /// <summary>
        /// Cast back to subclass type (bit messy this!)
        /// </summary>
        /// <returns></returns>
        protected virtual T End()
        {
            return this as T;
        }

        public override string ToString()
        {
            return myCmdBuilder.ToString();
        }

        /// <summary>
        /// This will append to <paramref name="paramValue"/> a wildcard character
        /// suitable for us in LIKE operations
        /// </summary>
        /// <param name="paramValue">The string value to append the wildcard to</param>
        /// <returns></returns>
        public static string CreateWildCardValue(string paramValue)
        {
            return CreateWildCardValue(paramValue, false, true);
        }

        /// <summary>
        /// This will wrap the <paramref name="paramValue"/> with wildcard characters. Use
        /// <paramref name="prefix"/> and <paramref name="postfix"/> to control where the
        /// wildcards are affixed.
        /// </summary>
        /// <param name="paramValue">The string value to decorate with wildcards</param>
        /// <param name="prefix">If true will prefix the value</param>
        /// <param name="postfix">If true will append a wildcard to the value</param>
        /// <returns></returns>
        public static string CreateWildCardValue(string paramValue, bool prefix, bool postfix)
        {
            var newValue = paramValue;

            if (prefix)
                newValue = ("%" + newValue.TrimStart('*', '%'));

            if (postfix)
                newValue = (newValue.TrimEnd('*', '%') + "%");

            return newValue;
        }

        /// <summary>
        /// This will automatically exchange a "null" for DBNull.Value 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual object NullCheck(object value)
        {
            return value ?? DBNull.Value;
        }
    }
}