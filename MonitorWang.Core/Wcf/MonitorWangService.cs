using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Wcf
{
    public class MonitorWangService : IMonitorWang
    {
        public void CaptureAgentStart(HealthCheckAgentStart session)
        {
            Messenger.Publish(session);
        }

        public void CaptureResult(HealthCheckResult result)
        {
            Messenger.Publish(result);
        }
    }
}