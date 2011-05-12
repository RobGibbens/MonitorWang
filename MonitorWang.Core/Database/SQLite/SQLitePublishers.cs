using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Interfaces.Magnum;
using MonitorWang.Core.Publishers;

namespace MonitorWang.Core.Database.SQLite
{
    public class SQLitePublisherBase : PublisherBase
    {
        protected readonly SQLiteConfiguration myConfig;

        public SQLitePublisherBase(SQLiteConfiguration config)
        {
            myConfig = config;

            Enabled = config.Enabled;
            FriendlyId = config.FriendlyId;
        }

        public override void Initialise()
        {
            var dbFile = ExtractFilename(SmartConnectionString.For(myConfig.ConnectionString));

            try
            {
                if (!File.Exists(dbFile))
                {
                    Logger.Debug("\tCreating MonitorWang SQLite datafile at {0}...", dbFile);
                    SQLiteConnection.CreateFile(dbFile);
                }

                var schema = GetSchema("AgentData");

                if (schema.Columns.Count == 0)
                {
                    Logger.Debug("\tCreating AgentData table...");
                    using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                        .WithSql(SQLiteStatement.Create("CREATE TABLE AgentData (")
                                     .Append("[TypeId] UNIQUEIDENTIFIER NOT NULL,")
                                     .Append("[EventType] TEXT NOT NULL,")
                                     .Append("[SiteId] TEXT NOT NULL,")
                                     .Append("[AgentId] TEXT NOT NULL,")
                                     .Append("[CheckId] TEXT NULL,")
                                     .Append("[Result] BOOL NULL,")
                                     .Append("[GeneratedOnUtc] DATETIME NOT NULL,")
                                     .Append("[ReceivedOnUtc] DATETIME NOT NULL,")
                                     .Append("[Data] TEXT NOT NULL,")
                                     .Append("[Tags] TEXT NULL,")
                                     .Append("[Version] UNIQUEIDENTIFIER NOT NULL,")
                                     .Append("[ResultCount] REAL NULL,")
                                     .Append("[MinuteBucket] INTEGER NULL,")
                                     .Append("[HourBucket] INTEGER NULL,")
                                     .Append("[DayBucket] INTEGER NULL)")))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                AddColumnIfMissing("MinuteBucket", "INTEGER", true);
                AddColumnIfMissing("HourBucket", "INTEGER", true);
                AddColumnIfMissing("DayBucket", "INTEGER", true);
                Logger.Debug("\tSuccess, AgentData table established");
            }
            catch (Exception)
            {
                Logger.Debug("\tError during SQLite datafile/database creation...");
                throw;
            }
        }

        protected virtual string ExtractFilename(string connectionString)
        {
            var parser = new DbConnectionStringBuilder { ConnectionString = connectionString };
            return parser["Data Source"].ToString();
        }

        protected virtual TableSchema GetSchema(string table)
        {
            var columns = new List<TableSchema.ColumnDefinition>();

            using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create("PRAGMA table_info('{0}')", table)))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var def = new TableSchema.ColumnDefinition
                                      {
                                          Name = reader["name"].ToString(),
                                          Type = reader["type"].ToString(),
                                          IsNullable = Convert.ToBoolean(reader["notnull"])
                                      };
                        columns.Add(def);
                    }
                }
            }

            return new TableSchema
                       {
                           Columns = columns.AsReadOnly()
                       };
        }

        protected virtual bool AddColumnIfMissing(string column, string datatype, bool nullable)
        {
            if (GetSchema("AgentData").HasColumn(column))
                return false;

            using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create("ALTER TABLE AgentData ADD {0} {1} {2} NULL",
                    column,
                    datatype,
                    nullable ? string.Empty : "NOT")))
            {
                cmd.ExecuteNonQuery();
                Logger.Debug("\tAdded column {0} [{1}]", column, datatype);
            }
            return true;
        }
    }

    public class SQLiteHealthCheckSessionPublisher : SQLitePublisherBase, IHealthCheckSessionPublisher
    {
        public SQLiteHealthCheckSessionPublisher(SQLiteConfiguration config)
            : base(config)
        {
        }

        public void Publish(HealthCheckAgentStart message)
        {
            var data = SerialisationHelper<HealthCheckAgentStart>.DataContractSerialize(message);

            using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create("INSERT INTO AgentData (")
                .Append("TypeId,EventType,SiteId,AgentId,GeneratedOnUtc,ReceivedOnUtc,Data,Version")
                .Append(") VALUES (")
                .InsertParameter("@pTypeId", message.Id).Append(",")
                .InsertParameter("@pEventType", "SessionStart").Append(",")
                .InsertParameter("@pSiteId", message.Agent.SiteId).Append(",")
                .InsertParameter("@pAgentId", message.Agent.AgentId).Append(",")
                .InsertParameter("@pGeneratedOnUtc", message.DiscoveryStarted).Append(",")
                .InsertParameter("@pReceivedOnUtc", DateTime.UtcNow).Append(",")
                .InsertParameter("@pData", data).Append(",")
                .InsertParameter("@pVersion", message.Id)
                .Append(")")))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void Consume(HealthCheckAgentStart message)
        {
            Publish(message);
        }
    }

    public class SQLiteHealthCheckResultPublisher : SQLitePublisherBase, IHealthCheckResultPublisher
    {
        public SQLiteHealthCheckResultPublisher(SQLiteConfiguration config)
            : base(config)
        {
        }

        public void Publish(HealthCheckResult message)
        {
            var data = SerialisationHelper<HealthCheckResult>.DataContractSerialize(message);

            using (var cmd = SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create("INSERT INTO AgentData (")
                .Append("TypeId,EventType,SiteId,AgentId,CheckId,Result,ResultCount,GeneratedOnUtc,ReceivedOnUtc,Data,Tags,Version,MinuteBucket,HourBucket,DayBucket")
                .Append(") VALUES (")
                .InsertParameter("@pTypeId", message.Check.Identity.TypeId).Append(",")
                .InsertParameter("@pEventType", message.EventType).Append(",")
                .InsertParameter("@pSiteId", message.Agent.SiteId).Append(",")
                .InsertParameter("@pAgentId", message.Agent.AgentId).Append(",")
                .InsertParameter("@pCheckId", message.Check.Identity.Name).Append(",")
                .InsertParameter("@pResult", message.Check.Result).Append(",")
                .InsertParameter("@pResultCount", message.Check.ResultCount).Append(",")
                .InsertParameter("@pGeneratedOnUtc", message.Check.GeneratedOnUtc).Append(",")
                .InsertParameter("@pReceivedOnUtc", DateTime.UtcNow).Append(",")
                .InsertParameter("@pData", data).Append(",")
                .InsertParameter("@pTags", message.Check.Tags).Append(",")
                .InsertParameter("@pVersion", message.Id).Append(",")
                .InsertParameter("@pMinuteBucket", message.MinuteBucket).Append(",")
                .InsertParameter("@pHourBucket", message.HourBucket).Append(",")
                .InsertParameter("@pDayBucket", message.DayBucket)
                .Append(")")))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public void Consume(HealthCheckResult message)
        {
            Publish(message);
        }
    }
}