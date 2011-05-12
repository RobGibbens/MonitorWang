using MonitorWang.Core.Interfaces.Entities;
using NServiceBus;

namespace MonitorWang.Core.Bus
{
    public class HealthCheckResultNotification : HealthCheckResult, IMessage
    {
    }
}