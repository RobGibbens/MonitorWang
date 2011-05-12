using System;

namespace MonitorWang.Core.Schedulers
{
    public static class DateTimeExtensions
    {
        public static DateTime SetTimeOfDay(this DateTime date, int hour, int minute, int second)
        {
            return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
        }
    }
}