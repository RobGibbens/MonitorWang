using System;

namespace MonitorWang.Core.AppStats
{
    public class AppStatsConfigBuilder
    {
        private AppStatsConfig myConfig;

        public Action<AppStatsConfig> PublisherInjector { get; set; }

        public static AppStatsConfigBuilder For(string applicationId)
        {
            return new AppStatsConfigBuilder
                       {
                           myConfig = new AppStatsConfig
                                        {
                                            ApplicationId = applicationId,
                                            MachineId = Environment.MachineName
                                        }
                       };
        }

        public AppStatsConfigBuilder MachineIdIs(string machineId)
        {
            myConfig.MachineId = machineId;
            return this;
        }

        public AppStatsConfig Build()
        {
            
            PublisherInjector.Invoke(myConfig);
            return myConfig;
        }
    }
}