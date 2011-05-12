using System;
using System.Collections.Generic;
using System.Data;
using MonitorWang.Core.Database;

namespace MonitorWang.Core.Geckoboard.DataProvider
{
    public abstract class SqlGeckoDataProviderBase : IGeckoboardDataProvider
    {
        public static readonly DateTime BucketBaselineDate = new DateTime(2011, 01, 01);

        public string FriendlyId { get; set;}

        protected abstract AdhocCommandBase GetPieChartDataForAllSitesCommand();
        public IEnumerable<PieChartData> GetPieChartDataForAllSites()
        {
            IEnumerable<PieChartData> data;

            using (var cmd = GetPieChartDataForAllSitesCommand())
            {
                data = GetPiechartData(cmd.ExecuteReader());
            }

            return data;

        }

        protected abstract AdhocCommandBase GetPieChartDataForSiteCommand(string site);
        public IEnumerable<PieChartData> GetPieChartDataForSite(string site)
        {
            IEnumerable<PieChartData> data;

            using (var cmd = GetPieChartDataForSiteCommand(site))
            {
                data = GetPiechartData(cmd.ExecuteReader());
            }

            return data;
        }

        protected abstract AdhocCommandBase GetGeckoboardPieChartForCheckCommand(PieChartArgs args);
        public IEnumerable<PieChartData> GetGeckoboardPieChartForCheck(PieChartArgs args)
        {
            IEnumerable<PieChartData> data;

            using (var cmd = GetGeckoboardPieChartForCheckCommand(args))
            {
                data = GetPiechartData(cmd.ExecuteReader());
            }

            return data;
        }

        protected IEnumerable<PieChartData> GetPiechartData(IDataReader reader)
        {
            var data = new List<PieChartData>();

            using (reader)
            {
                while (reader.Read())
                {
                    data.Add(new PieChartData
                    {
                        Label = reader["SegmentId"].ToString(),
                        Count = Convert.ToInt64(reader["Count"])
                    });
                }
            }

            return data;            
        }

        protected abstract AdhocCommandBase GetLineChartDataForCheckRateCommand(LineChartArgs args);
        public IEnumerable<LineChartData> GetLineChartDataForCheckRate(LineChartArgs args)
        {
            var data = new List<LineChartData>();

            switch (args.Unit.ToLowerInvariant())
            {
                case "hour":
                    args.Bucket = "hourbucket";
                    args.Multiplier = 60;
                    break;
                case "day":
                    args.Bucket = "daybucket";
                    args.Multiplier = 1440;
                    break;
                default:
                    args.Bucket = "minutebucket";
                    args.Multiplier = 1;
                    break;
            }

            using (var cmd = GetLineChartDataForCheckRateCommand(args))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var minutes = Convert.ToInt32(reader["MinutesBucket"]);
                        var date = BucketBaselineDate.AddMinutes(minutes);

                        data.Add(new LineChartData
                                     {
                                         Value = Convert.ToDouble(reader["ResultCount"]),
                                         When = Convert.ToDateTime(date)
                                     });
                    }
                }
            }

            return data;
        }

        protected abstract AdhocCommandBase GetGeckoMeterDataForSiteCheckCommand(GeckometerArgs args);
        public GeckometerData GetGeckoMeterDataForSiteCheck(GeckometerArgs args)
        {
            var data = new GeckometerData();

            using (var cmd = GetGeckoMeterDataForSiteCheckCommand(args))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data.Avg = (reader["Avg"] == DBNull.Value)
                                        ? 0
                                        : Convert.ToDouble(reader["Avg"]);
                        data.Min = (reader["Min"] == DBNull.Value)
                                             ? 0
                                             : Convert.ToDouble(reader["Min"]);
                        data.Max = (reader["Max"] == DBNull.Value)
                                             ? 0
                                             : Convert.ToDouble(reader["Max"]);
                    }
                }
            }

            return data;
        }

        protected abstract AdhocCommandBase GetComparisonDataForSiteCheckCommand(ComparisonArgs args);
        public ComparisonData GetComparisonDataForSiteCheck(ComparisonArgs args)
        {
            var data = new ComparisonData();

            using (var cmd = GetComparisonDataForSiteCheckCommand(args))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    var row = 0;

                    while (reader.Read())
                    {
                        if (++row == 1)
                            data.Number = Convert.ToDouble(reader["ResultCount"]);
                        else
                            data.Comparison = Convert.ToDouble(reader["ResultCount"]);
                    }
                }
            }

            return data;
        }

        protected virtual string ConvertOutcome(OutcomeType type)
        {
            var outcome = string.Empty;

            switch (type)
            {
                case OutcomeType.Failure:
                    outcome = "(Result = 0)";
                    break;
                case OutcomeType.Success:
                    outcome = "(Result = 1)";
                    break;
            }

            return outcome;
        }

        protected virtual string ConvertOperation(DataOperationType type)
        {
            var operation = "COUNT(*)";

            switch (type)
            {
                case DataOperationType.Average:
                    operation = "AVG(ResultCount)";
                    break;
                case DataOperationType.Sum:
                    operation = "SUM(ResultCount)";
                    break;
                case DataOperationType.Max:
                    operation = "MAX(ResultCount)";
                    break;
                case DataOperationType.Min:
                    operation = "MIN(ResultCount)";
                    break;
            }

            return operation;
        }
    }
}