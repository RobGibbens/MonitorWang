using System;
using System.Reflection;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;
using NServiceBus;

namespace MonitorWang.Core.Bus
{
    public class BusBridgeStartup : IStartupPlugin, ICanBeSwitchedOff
    {
        protected readonly BusConfig myConfig;

        public BusBridgeStartup(BusConfig config)
        {
            myConfig = config;
        }

        public bool Enabled
        {
            get { return myConfig.Enabled; }
            set { myConfig.Enabled = value; }
        }

        public Status Status { get; set; }

        public void Initialise()
        {
            if (!Enabled)
                return;

            if (Container.IsRegistered<IBus>())
                throw new InvalidOperationException(string.Format("An instance of the NSB Bus is already registered. Only one Bus can be registered so this indicates that you are trying to run an Agent as both a Bus Publisher AND a Bridge; an Agent can only act as one, please check the BusBridge (activity) and BusPublisher (publisher) configs and disable one"));

            RegisterBus();
        }

        protected virtual void RegisterBus()
        {
            var bus = GetBasicBus();

            bus.UnicastBus().LoadMessageHandlers()
                .CreateBus().Start();            
            
        }

        protected virtual Configure GetBasicBus()
        {
            return Container.Bus(Assembly.GetAssembly(typeof(HealthCheckAgentStartNotification)).FullName)
                .CustomConfigurationSource(new BusZeroConfig(myConfig))
                .Log4Net()
                .XmlSerializer()
                .MsmqTransport().IsTransactional(true);
        }
    }

    public class BusPublisherStartup : BusBridgeStartup
    {
        public BusPublisherStartup(BusPublisherConfig config) 
            : base(config)
        {
        }

        protected override void RegisterBus()
        {
            // load the bus without loading message handlers - this
            // stops the bus from acting as a bridge as it can never
            // handle received messages
            GetBasicBus().UnicastBus().CreateBus().Start();
        }
    }
}