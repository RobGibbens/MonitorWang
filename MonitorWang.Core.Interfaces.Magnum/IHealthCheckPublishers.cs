using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Interfaces.Magnum
{
    /// <summary>
    /// Allows us to persist information about the start of a health 
    /// check session eg: what checks are running
    /// </summary>
    public interface IHealthCheckSessionPublisher : IPublisherPlugin<HealthCheckAgentStart>
    {
    }

    /// <summary>
    /// Allows us to persist the result of a health check
    /// </summary>
    public interface IHealthCheckResultPublisher : IPublisherPlugin<HealthCheckResult>
    {
    }
}