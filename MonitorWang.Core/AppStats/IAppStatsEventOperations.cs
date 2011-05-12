using System;

namespace MonitorWang.Core.AppStats
{
    public interface IAppStatsEventOperations<T>
    {
        T One();
        T Count(double count);
        T Time(TimeSpan duration);
    }
}