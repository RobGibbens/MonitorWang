using System;

namespace MonitorWang.Core.AppStats
{
    public class AppStatsEvent : AppStatsEventExtensions.IAppStatsPieChartContinuation<AppStatsEvent>
    {
        public static readonly DateTime BucketBaselineDate = new DateTime(2011, 01, 01);

        public string SiteId { get; set; }
        public string AgentId { get; set; }
        public string CheckId { get; set; }
        public string Tag { get; set; }
        public TimeSpan? Duration { get; set; }
        public double? ResultCount { get; set; }
        public int? MinuteBucket { get; set; }
        public int? HourBucket { get; set; }
        public int? DayBucket { get; set; }

        public AppStatsEvent()
        {
            MinuteBucket = (int)DateTime.UtcNow.Subtract(BucketBaselineDate).TotalMinutes;
            HourBucket = (int)DateTime.UtcNow.Subtract(BucketBaselineDate).TotalHours;
            DayBucket = (int)DateTime.UtcNow.Subtract(BucketBaselineDate).TotalDays;
        }

        public AppStatsEvent One()
        {
            return Count(1);            
        }

        public AppStatsEvent Count(double count)
        {
            ResultCount = count;
            return this;
        }

        public AppStatsEvent Time(TimeSpan duration)
        {
            Duration = duration;
            ResultCount = duration.TotalMilliseconds;
            return this;
        }

        AppStatsEvent AppStatsEventExtensions.IAppStatsPieChartContinuation<AppStatsEvent>.Segment(string id)
        {
            Tag = id;
            return this;
        }
    }
}