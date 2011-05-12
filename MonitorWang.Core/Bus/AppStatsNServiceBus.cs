using MonitorWang.Core.AppStats;
using NServiceBus;

namespace MonitorWang.Core.Bus
{
    public static class AppStatsNServiceBus
    {
        public static AppStatsConfigBuilder PublishWith(this AppStatsConfigBuilder builder, IBus bus)
        {
            builder.PublisherInjector = (config => config.Publisher = new AppStatNServiceBusPublisher(bus));
            return builder;
        }

        public static AppStatsConfigBuilder PublishWith(this AppStatsConfigBuilder builder, IBus bus, string destinationQueue)
        {
            builder.PublisherInjector = (config => config.Publisher = new AppStatNServiceBusPublisher(bus,
                destinationQueue));
            return builder;
        }
    }
}