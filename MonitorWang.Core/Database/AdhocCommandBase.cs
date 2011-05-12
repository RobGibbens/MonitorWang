using System;
using System.Collections.Generic;
using System.Data;

namespace MonitorWang.Core.Database
{
    /// <summary>
    /// Fluent wrapper to core data access components to execute an ad-hoc (inline) sql statement
    /// </summary>
    public abstract class AdhocCommandBase : IDisposable
    {
        public Dictionary<string, IDataParameter> Parameters;

        protected bool myServerPrepare;
        protected string mySql;
        protected IDbConnection myConnection;
        protected IDbCommand myCommand;
        protected string myConnectionString;

        private bool disposed; // to detect redundant dispose calls

        protected AdhocCommandBase()
        {
            Parameters = new Dictionary<string, IDataParameter>();
        }

        /// <summary>
        /// Prepares this command on the server - improves performance if executed
        /// many times as it just swaps the parameters values as they change. Requires
        /// parameters to be explicitly defined (type and size)
        /// </summary>
        /// <returns></returns>
        public AdhocCommandBase PrepareOnServer()
        {
            myServerPrepare = true;
            return this;
        }

        /// <summary>
        /// Sets the sql statement to execute
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public AdhocCommandBase WithSql(string statement, params object[] args)
        {
            mySql = string.Format(statement, args);
            return this;
        }

        /// <summary>
        /// Sets the sql statement to execute from the <see cref="ISqlStatement"/>
        /// fluent interface. Any parameters set on the SqlStatement will 
        /// replace any existing set of parameters.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public AdhocCommandBase WithSql(ISqlStatement sql)
        {
            mySql = sql.ToString();
            Parameters = sql.Parameters;
            return this;
        }

        /// <summary>
        /// Adds parameter(s) to the command. This is recommended when the command
        /// takes external values as part of the command sql as there are much
        /// lower security risks than inline parameter values using strings directly
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public AdhocCommandBase AddParameters(params IDataParameter[] parameters)
        {
            foreach (var parameter in parameters)
                Parameters.Add(parameter.ParameterName, parameter);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract IDbConnection NewConnection();

        /// <summary>
        /// Creates a database command object to execute
        /// </summary>
        /// <returns></returns>
        public IDbCommand Build()
        {
            if (ConnectionIsActive())
                throw new InvalidOperationException(string.Format("Database connection '{0}' is active; this state is invalid to execute the Build command", myConnectionString));

            myConnection = NewConnection();
            myConnection.ConnectionString = myConnectionString;
            myConnection.Open();

            myCommand = myConnection.CreateCommand();
            myCommand.CommandType = CommandType.Text;

            if (!string.IsNullOrEmpty(mySql))
                myCommand.CommandText = mySql;

            foreach (var param in Parameters)
                myCommand.Parameters.Add(param.Value);

            if (myServerPrepare)
                myCommand.Prepare();

            return myCommand;
        }

        /// <summary>
        /// Creates the command and executes it to return a reader with 
        /// firehose (r/o, f/o) access to the data returned
        /// </summary>
        /// <returns></returns>
        public IDataReader ExecuteReader()
        {
            return ConnectionIsActive() ? myCommand.ExecuteReader() : Build().ExecuteReader();
        }

        /// <summary>
        /// Creates the command and executes it 
        /// </summary>
        public int ExecuteNonQuery()
        {
            return ConnectionIsActive() ? myCommand.ExecuteNonQuery() : Build().ExecuteNonQuery();
        }

        /// <summary>
        /// Creates the command and executes it and returns the first 
        /// column of the first row in the resultset
        /// </summary>
        public object ExecuteScalar()
        {
            return ConnectionIsActive() ? myCommand.ExecuteScalar() : Build().ExecuteScalar();
        }

        /// <summary>
        /// This will set the value of a parameter on an existing command object.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public AdhocCommandBase SetParameterValue(string paramName, object value)
        {
            Parameters[paramName].Value = value ?? DBNull.Value;
            return this;
        }

        protected bool ConnectionIsActive()
        {
            if (myConnection == null)
                return false;
            return (myConnection.State != ConnectionState.Closed);
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        /// <remarks>Disposable pattern courtesy of http://www.codeproject.com/KB/dotnet/idisposable.aspx</remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This will clean up the connection and command associated with this SP
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Explicit call to dispose resources - eg: using statement
                    // Dispose managed resources.
                    if (myCommand != null)
                        myCommand.Dispose();

                    if (myConnection != null)
                        myConnection.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            disposed = true;
        }

    }
}