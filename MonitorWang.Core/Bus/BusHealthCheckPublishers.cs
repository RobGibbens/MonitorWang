using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Interfaces.Magnum;
using MonitorWang.Core.Publishers;
using NServiceBus;

namespace MonitorWang.Core.Bus
{
    /// <summary>
    /// This will publish health check session start messages to the bus
    /// </summary>
    public class BusHealthCheckSessionPublisher : PublisherBase, IHealthCheckSessionPublisher
    {
        protected readonly IBus myBus;
        protected readonly BusPublisherConfig myConfig;

        public BusHealthCheckSessionPublisher(BusPublisherConfig config,
            IBus bus)
        {            
            myBus = bus;
            myConfig = config;

            Enabled = config.Enabled;
        }

        public void Publish(HealthCheckAgentStart message)
        {
            myBus.Send<HealthCheckAgentStartNotification>(myConfig.OutputQueue,
                m =>
                {
                    m.Agent = message.Agent;
                    m.Id = message.Id;
                    m.Checks = message.Checks;
                    m.Activities = message.Activities;
                    m.DiscoveryCompleted = message.DiscoveryCompleted;
                    m.DiscoveryStarted = message.DiscoveryStarted;

                    if (!string.IsNullOrEmpty(myConfig.HttpGatewayUri))
                        m.SetHttpToHeader(myConfig.HttpGatewayUri);
                });
        }

        public void Consume(HealthCheckAgentStart message)
        {
            Publish(message);
        }
    }

    /// <summary>
    /// This will publish health check result messages to the bus
    /// </summary>
    public class BusHealthCheckResultPublisher : PublisherBase, IHealthCheckResultPublisher
    {
        protected readonly IBus myBus;
        protected readonly BusPublisherConfig myConfig;

        public BusHealthCheckResultPublisher(BusPublisherConfig config, IBus bus)
        {
            myBus = bus;
            myConfig = config;

            Enabled = config.Enabled;
            FriendlyId = config.FriendlyId;
        }

        public void Publish(HealthCheckResult message)
        {
            myBus.Send<HealthCheckResultNotification>(myConfig.OutputQueue,
                m =>
                {
                    m.Id = message.Id;
                    m.Agent = message.Agent;
                    m.Check = message.Check;

                    if (!string.IsNullOrEmpty(myConfig.HttpGatewayUri))
                        m.SetHttpToHeader(myConfig.HttpGatewayUri);
                });
        }

        public void Consume(HealthCheckResult message)
        {
            Publish(message);
        }
    }
}