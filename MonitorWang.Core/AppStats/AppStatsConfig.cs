namespace MonitorWang.Core.AppStats
{
    public class AppStatsConfig
    {
        public IAppStatsPublisher Publisher { get; set; }
        public string MachineId { get; set; }
        public string ApplicationId { get; set; }
    }
}