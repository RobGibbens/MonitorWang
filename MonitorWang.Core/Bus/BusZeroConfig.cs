using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

namespace MonitorWang.Core.Bus
{
    /// <summary>
    /// This should remove the need for NSB xml configuration
    /// </summary>
    /// <remarks>http://www.candland.net/blog/2009/09/08/OverridingNServiceBusConfiguration.aspx</remarks>
    public class BusZeroConfig : IConfigurationSource
    {
        protected readonly BusConfig myConfig;

        public BusZeroConfig(BusConfig config)
        {
            myConfig = config;
        }

        public T GetConfiguration<T>() where T : class
        {
            if (typeof(T) == typeof(MsmqTransportConfig))
                return new MsmqTransportConfig
                           {
                               ErrorQueue = myConfig.ErrorQueue,
                               MaxRetries = 5,
                               InputQueue = myConfig.InputQueue,
                               NumberOfWorkerThreads = 1                                                             
                           } as T;

            if (typeof(T) == typeof(UnicastBusConfig))
                return new UnicastBusConfig() as T;

            return null;
        }
    }
}