using System;
using MonitorWang.Core;
using MonitorWang.Core.Growl;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Castle;
using MonitorWang.Core.Interfaces.Magnum;
using MonitorWang.Core.Loaders;

namespace MonitorWang.Agent.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultAgentProfile : ProfileBase
    {
        public override string Name
        {
            get { return "DefaultAgent"; }
        }

        public virtual Type DefineRole()
        {
            return typeof (Roles.Agent);
        }

        public override void CreateRole()
        {            
            Container.RegisterAllWithInterception<IGrowlNotificationFinaliser, IGrowlNotificationFinaliserInterceptor>()
                .RegisterAll<IHealthCheckSessionPublisher>()
                .RegisterAllWithInterception<IHealthCheckResultPublisher, IPublisherFilter>()
                .RegisterAll<IActivityPlugin>()                
                .RegisterAsSingleton<ILoader<IHealthCheckSchedulerPlugin>>(typeof(HealthCheckLoader))
                .RegisterAsSingleton<ILoader<IActivityPlugin>>(typeof(ContainerPluginLoader<IActivityPlugin>))
                .RegisterAsSingleton<ILoader<IHealthCheckSessionPublisher>>(typeof(ContainerPluginLoader<IHealthCheckSessionPublisher>))
                .RegisterAsSingleton<ILoader<IHealthCheckResultPublisher>>(typeof(ContainerPluginLoader<IHealthCheckResultPublisher>))
                .RegisterAsSingleton<IRolePlugin>(DefineRole());

            Messenger.Initialise(new MagnumMessenger());
        }
    }
}