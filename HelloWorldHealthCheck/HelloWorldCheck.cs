using System;
using MonitorWang.Core;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace HelloWorldHealthCheck
{
    public class HelloWorldCheckConfig : PluginConfigBase
    {
        public string MyCustomSetting { get; set; }
    }

    public class HelloWorldCheck : IHealthCheckPlugin
    {
        private readonly HelloWorldCheckConfig myConfig;

        public HelloWorldCheck(HelloWorldCheckConfig config)
        {
            myConfig = config;
        }

        public void Initialise()
        {
            // This can be used to do any "one-time" initialisation
            // of this check - this method is called when the plugin
            // is loaded (must also be "enabled" in its configuration)
        }

        public Status Status { get; set; }

        public PluginDescriptor Identity
        {
            get { return new PluginDescriptor
                             {
                                 Description = "My first custom HealthCheck!",
                                 Name = myConfig.FriendlyId,
                                 // The TypeId is important - it needs to be different for
                                 // each health check as this allows us to positively identify
                                 // every health check. Use the VS Tools/Create GUID tool to
                                 // generate a new one.
                                 TypeId = new Guid("218087BB-3605-4fa5-9157-0C133674F51F")
                             }; }
        }

        public void Execute()
        {
            Messenger.Publish(new HealthCheckData
                        {
                            Identity = Identity,
                            Result = true,
                            Info = string.Format("MyCustomSetting:={0}", myConfig.MyCustomSetting)
                        });
        }
    }
}
