
using MonitorWang.Core.Database;
using MonitorWang.Core.Database.SqlServer;

namespace MonitorWang.Core.Geckoboard.DataProvider
{
    public class SqlServerGeckoDataProviderConfig
    {
        public string ConnectionString { get; set; }
    }

    public class SqlServerGeckoDataProvider : SqlGeckoDataProviderBase
    {
        protected SqlServerGeckoDataProviderConfig myConfig;

        /// <summary>
        /// Default ctor
        /// </summary>
        public SqlServerGeckoDataProvider()
        {
            FriendlyId = string.Format("{0}SqlServer", GeckoboardDataService.GeckoboardDataProviderPrefix);
            myConfig = new SqlServerGeckoDataProviderConfig
                           {
                               ConnectionString = "MonitorWang"
                           };
        }

        /// <summary>
        /// Allows external configuration (config object must be in IoC Container
        /// or created by hand)
        /// </summary>
        /// <param name="config"></param>
        public SqlServerGeckoDataProvider(SqlServerGeckoDataProviderConfig config)
            : this()
        {
            myConfig = config;
        }

        protected override AdhocCommandBase GetPieChartDataForAllSitesCommand()
        {
            return SqlServerAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SqlServerStatement.Create(
                        "SELECT SiteId [SegmentId], COUNT(*) [Count] FROM AgentData WHERE Result = 0 AND EventType = 'Result' GROUP BY SiteId"));
        }

        protected override AdhocCommandBase GetPieChartDataForSiteCommand(string site)
        {
            return SqlServerAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SqlServerStatement.Create(
                    "SELECT CheckId [SegmentId], COUNT(*) [Count] FROM AgentData WHERE (Result = 0) AND (EventType = 'Result') AND (lower(SiteId)=")
                             .InsertParameter("@pSiteId", site.ToLower())
                             .Append(") GROUP BY CheckId"));
        }

        protected override AdhocCommandBase GetGeckoboardPieChartForCheckCommand(PieChartArgs args)
        {
            string outcome;
            string operation;

            switch (args.DataOperation)
            {
                case DataOperationType.Average:
                    operation = "AVG(ResultCount)";
                    break;
                case DataOperationType.Sum:
                    operation = "SUM(ResultCount)";
                    break;
                default:
                    operation = "COUNT(*)";
                    break;
            }

            switch (args.Outcome)
            {
                case OutcomeType.Failure:
                    outcome = "(Result = 0)";
                    break;
                case OutcomeType.Success:
                    outcome = "(Result = 1)";
                    break;
                default:
                    outcome = string.Empty;
                    break;
            }
            
            return SqlServerAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SqlServerStatement.Create(
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

            return SqlServerAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SqlServerStatement.Create(
                    "SELECT TOP {0} ({1}*{2}) [MinutesBucket], {3} [ResultCount] FROM AgentData WHERE CheckId=",
                    args.Limit,
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
                             .OrderBy(args.Bucket));
        }

        protected override AdhocCommandBase GetGeckoMeterDataForSiteCheckCommand(GeckometerArgs args)
        {
            return SqlServerAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SqlServerStatement.Create(
                    "SELECT MIN(ResultCount) [Min], MAX(ResultCount) [Max], AVG(ResultCount) [Avg] FROM AgentData WHERE (ResultCount IS NOT NULL) AND (EventType = 'Result') AND (SiteId=")
                             .InsertParameter("@pSiteId", args.Site)
                             .Append(") AND (CheckId=")
                             .InsertParameter("@pCheckId", args.Check).Append(")")
                             .AppendIf(() => !string.IsNullOrEmpty(args.Tag), "AND (tag='{0}')", args.Tag));
        }

        protected override AdhocCommandBase GetComparisonDataForSiteCheckCommand(ComparisonArgs args)
        {
            return SqlServerAdhocCommand.UsingSmartConnection(myConfig.ConnectionString)
                .WithSql(SqlServerStatement.Create(
                    "SELECT TOP 2 ResultCount FROM AgentData WHERE (ResultCount IS NOT NULL) AND (EventType = 'Result') AND (SiteId=")
                             .InsertParameter("@pSiteId", args.Site)
                             .Append(") AND (CheckId=")
                             .InsertParameter("@pCheckId", args.Check).Append(")")
                             .AppendIf(() => !string.IsNullOrEmpty(args.Tag), "AND (tag='{0}')", args.Tag)
                             .OrderBy("GeneratedOnUtc"));
        }
    }
}