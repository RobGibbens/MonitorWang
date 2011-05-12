
using MonitorWang.Core.Database;
using MonitorWang.Core.Database.SQLite;

namespace MonitorWang.Core.Geckoboard.DataProvider
{
    public class SQLiteGeckoDataProviderConfig
    {
        public string ConnectionString { get; set; }
    }

    public class SQLiteGeckoDataProvider : SqlGeckoDataProviderBase
    {
        protected SQLiteGeckoDataProviderConfig myConfig;

        /// <summary>
        /// Default ctor
        /// </summary>
        public SQLiteGeckoDataProvider()
        {
            FriendlyId = string.Format("{0}SQLite", GeckoboardDataService.GeckoboardDataProviderPrefix);
            myConfig = new SQLiteGeckoDataProviderConfig
                           {
                               ConnectionString = "MonitorWang-SQLite"
                           };
        }

        /// <summary>
        /// Allows external configuration (config object must be in IoC Container
        /// or created by hand)
        /// </summary>
        /// <param name="config"></param>
        public SQLiteGeckoDataProvider(SQLiteGeckoDataProviderConfig config)
            : this()
        {
            myConfig = config;
        }

        protected override AdhocCommandBase GetPieChartDataForAllSitesCommand()
        {
            return SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create("SELECT SiteId, COUNT(*) [Failures] FROM AgentData WHERE Result = 0 AND EventType = 'Result' GROUP BY SiteId"));
        }

        protected override AdhocCommandBase GetPieChartDataForSiteCommand(string site)
        {
            return SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create(
                    "SELECT CheckId, COUNT(*) [Failures] FROM AgentData WHERE (Result = 0) AND (EventType = 'Result') AND (lower(SiteId)=")
                             .InsertParameter("@pSiteId", site.ToLower())
                             .Append(") GROUP BY CheckId"));
        }

        protected override AdhocCommandBase GetGeckoboardPieChartForCheckCommand(PieChartArgs args)
        {
            var outcome = ConvertOutcome(args.Outcome);
            var operation = ConvertOperation(args.DataOperation);

            return SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create(
                    "SELECT Tags [SegmentId], {0} [Count] FROM AgentData WHERE CheckId=", operation)
                    .InsertParameter("@pCheckId", args.Check)
                    .AppendIf(() => !string.IsNullOrEmpty(outcome), "AND {0}", outcome)
                    .AppendIf(() => !string.IsNullOrEmpty(args.Site), "AND SiteId=")
                    .InsertParameterIf(() => !string.IsNullOrEmpty(args.Site), "@pSiteId", args.Site)
                    .AppendIf(() => !string.IsNullOrEmpty(args.Agent), "AND AgentId=")
                    .InsertParameterIf(() => !string.IsNullOrEmpty(args.Agent), "@pAgentId", args.Agent)
                    .Append("GROUP BY Tags"));

        }

        protected override AdhocCommandBase GetLineChartDataForCheckRateCommand(LineChartArgs args)
        {
            var outcome = ConvertOutcome(args.Outcome);
            var operation = ConvertOperation(args.DataOperation);

            return SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create(
                    "SELECT ({0}*{1}) [MinutesBucket], {2} [ResultCount] FROM AgentData WHERE CheckId=",
                    args.Bucket,
                    args.Multiplier,
                    operation)
                             .InsertParameter("@pCheckId", args.Check)
                             .AppendIf(() => !string.IsNullOrEmpty(outcome), "AND {0}", outcome)
                             .AppendIf(() => !string.IsNullOrEmpty(args.Site), "AND SiteId=")
                             .InsertParameterIf(() => !string.IsNullOrEmpty(args.Site), "@pSiteId", args.Site)
                             .AppendIf(() => !string.IsNullOrEmpty(args.Agent), "AND AgentId=")
                             .InsertParameterIf(() => !string.IsNullOrEmpty(args.Agent), "@pAgentId", args.Agent)
                             .AppendIf(() => !string.IsNullOrEmpty(args.Tag), "AND tag=")
                             .InsertParameterIf(() => !string.IsNullOrEmpty(args.Tag), "@pTag", args.Tag)
                             .Append("GROUP BY {0}", args.Bucket)
                             .OrderBy(args.Bucket)
                             .Append("LIMIT {0}", args.Limit));
        }

        protected override AdhocCommandBase GetGeckoMeterDataForSiteCheckCommand(GeckometerArgs args)
        {
            return SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create(
                    "SELECT MIN(ResultCount) [Min], MAX(ResultCount) [Max], AVG(ResultCount) [Avg] FROM AgentData WHERE (ResultCount IS NOT NULL) AND (EventType = 'Result') AND (lower(SiteId)=")
                             .InsertParameter("@pSiteId", args.Site.ToLower())
                             .Append(") AND (lower(CheckId)=")
                             .InsertParameter("@pCheckId", args.Check.ToLower()).Append(")")
                             .AppendIf(() => !string.IsNullOrEmpty(args.Tag), "AND (tag='{0}')", args.Tag));
        }

        protected override AdhocCommandBase GetComparisonDataForSiteCheckCommand(ComparisonArgs args)
        {
            return SQLiteAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SQLiteStatement.Create(
                    "SELECT ResultCount FROM AgentData WHERE (ResultCount IS NOT NULL) AND (EventType = 'Result') AND (lower(SiteId)=")
                             .InsertParameter("@pSiteId", args.Site.ToLower())
                             .Append(") AND (lower(CheckId)=")
                             .InsertParameter("@pCheckId", args.Check.ToLower()).Append(")")
                             .AppendIf(() => !string.IsNullOrEmpty(args.Tag), "AND (tag='{0}')", args.Tag)
                             .OrderBy("GeneratedOnUtc")
                             .Append("LIMIT 2"));
        }
    }
}