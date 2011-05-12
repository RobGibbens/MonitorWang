using AutoMapper;
using MonitorWang.Core.Interfaces.Entities;
using NServiceBus;

namespace MonitorWang.Core.Bus
{
    /// <summary>
    /// This NSB handler acts as a simple pass through, it just invokes its
    /// publishers to persist the message data. This would typically be used by 
    /// a server to republish the monitoring data received via NSB
    /// </summary>
    public class PublishAgentSessionHandler : IMessageHandler<HealthCheckAgentStartNotification>
    {
        static PublishAgentSessionHandler()
        {
            Mapper.CreateMap<HealthCheckAgentStartNotification, HealthCheckAgentStart>();
        }

        public void Handle(HealthCheckAgentStartNotification message)
        {            
            Messenger.Publish(Mapper.Map<HealthCheckAgentStartNotification, HealthCheckAgentStart>(message));
        }
    }
    
    /// <summary>
    /// This NSB handler acts as a simple pass through, it just invokes its
    /// publishers to persist the message data. This would typically be used by 
    /// a server to republish the monitoring data received via NSB
    /// </summary>
    public class PublishAgentResultHandler : IMessageHandler<HealthCheckResultNotification>
    {
        static PublishAgentResultHandler()
        {
            Mapper.CreateMap<HealthCheckResultNotification, HealthCheckResult>();
        }

        public void Handle(HealthCheckResultNotification message)
        {
            Messenger.Publish(Mapper.Map<HealthCheckResultNotification, HealthCheckResult>(message));
        }
    }
}