using System.ServiceModel;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Interfaces.Magnum;
using MonitorWang.Core.Publishers;

namespace MonitorWang.Core.Wcf
{
    public class WcfSessionPublisher : PublisherBase, IHealthCheckSessionPublisher
    {        
        protected readonly WcfPublisherConfiguration myConfig;
        protected ChannelFactory<IMonitorWang> myFactory;

        public WcfSessionPublisher(WcfPublisherConfiguration config)
        {
            myConfig = config;

            Enabled = myConfig.Enabled;
            FriendlyId = myConfig.FriendlyId;
        }

        public void Publish(HealthCheckAgentStart message)
        {
            IMonitorWang proxy = null;

            try
            {
                if (myFactory == null)
                {
                    myFactory = new ChannelFactory<IMonitorWang>(
                        new BasicHttpBinding(),
                        myConfig.Uri
                        );
                }

                proxy = myFactory.CreateChannel();
                proxy.CaptureAgentStart(message);
                ((IClientChannel)proxy).Close();
            }
            catch
            {
                if ((proxy != null) && (((IClientChannel)proxy).State == CommunicationState.Faulted))
                    ((IClientChannel)proxy).Abort();

                throw;
            }
        }

        public void Consume(HealthCheckAgentStart message)
        {
            Publish(message);
        }
    }

    public class WcfResultPublisher : PublisherBase, IHealthCheckResultPublisher
    {
        protected ChannelFactory<IMonitorWang> myFactory;
        protected readonly WcfPublisherConfiguration myConfig;

        public WcfResultPublisher(WcfPublisherConfiguration config)
        {
            myConfig = config;

            Enabled = myConfig.Enabled;
            FriendlyId = myConfig.FriendlyId;
        }

        public void Publish(HealthCheckResult message)        
        {
            IMonitorWang proxy = null;

            try
            {
                if (myFactory == null)
                {
                    myFactory = new ChannelFactory<IMonitorWang>(
                        new BasicHttpBinding(),
                        myConfig.Uri);
                }

                proxy = myFactory.CreateChannel();
                proxy.CaptureResult(message);
                ((IClientChannel)proxy).Close();
            }
            catch
            {
                if ((proxy != null) && (((IClientChannel)proxy).State == CommunicationState.Faulted))
                    ((IClientChannel)proxy).Abort();

                throw;
            }                        
        }

        public void Consume(HealthCheckResult message)
        {
            Publish(message);
        }
    }
}