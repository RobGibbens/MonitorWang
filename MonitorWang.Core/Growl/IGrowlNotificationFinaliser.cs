using Castle.DynamicProxy;
using Growl.Connector;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Growl
{
    public interface IGrowlNotificationFinaliser
    {
        void Finalise(HealthCheckResult result, Notification notification);
    }

    public interface IGrowlNotificationFinaliserInterceptor : IInterceptor
    {
    }
}