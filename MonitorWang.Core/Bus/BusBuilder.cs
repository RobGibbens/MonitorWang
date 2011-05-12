using System;
using Castle.Windsor;
using Magnum;
using NServiceBus;

namespace MonitorWang.Core.Bus
{
    /// <summary>
    /// This class is responsible for building the configuration 
    /// for an NServiceBus instance
    /// </summary>
    public class BusBuilder
    {
        private Configure myConfiguration;
        public Action<Configure> MsmqSettingsInjector;
        public Action<Configure> ContainerInjector;

        private BusBuilder()
        {
            this.Msmq("AppStatsInput", "AppStatsError");
        }

        public static BusBuilder ForWebApplication()
        {
            return new BusBuilder
                       {
                           myConfiguration = Configure.WithWeb()
                       };
        }

        public static BusBuilder ForApplication()
        {
            return new BusBuilder
            {
                myConfiguration = Configure.With()
            };
        }

        public IBus FireItUp()
        {
            Guard.AgainstNull(MsmqSettingsInjector);

            if (ContainerInjector != null)
                ContainerInjector(myConfiguration);
            else
                myConfiguration.DefaultBuilder();

            MsmqSettingsInjector(myConfiguration);
            // default other transport settings...
            myConfiguration.XmlSerializer().UnicastBus();

            var bus = myConfiguration.CreateBus();
            return bus.Start();            
        }
    }

    public static class BusBuilderExtensions
    {
        public static BusBuilder Msmq(this BusBuilder builder, string inputQueue, string errorQueue)
        {
            builder.MsmqSettingsInjector = config =>
                                            {
                                                config.CustomConfigurationSource(new BusZeroConfig(new BusConfig
                                                {
                                                    ErrorQueue = errorQueue,
                                                    InputQueue = inputQueue
                                                }));

                                                config.MsmqTransport().IsTransactional(true);
                                            };

            return builder;            
        }

        public static BusBuilder UseContainer(this BusBuilder builder, IWindsorContainer container)
        {
            builder.ContainerInjector = config =>
                                            {
                                                config.CastleWindsorBuilder(container);
                                            };
            return builder;            
        }
    }
}