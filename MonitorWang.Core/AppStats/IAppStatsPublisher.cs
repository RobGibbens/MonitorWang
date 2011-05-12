
namespace MonitorWang.Core.AppStats
{
    public interface IAppStatsPublisher
    {
        void Publish(AppStatsEvent stat);
    }
}