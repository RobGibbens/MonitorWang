using Castle.DynamicProxy;
using Growl.Connector;
using MonitorWang.Core.Containers;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Growl
{
    /// <summary>
    /// This base class provides the infrastructure for a Growl Notification Finaliser. It provides
    /// the logic to see if a finaliser applies to a HealthCheck result - use this as a base class 
    /// for any new Finaliser you need.
    /// </summary>
    public abstract class GrowlNotificationInterceptorBase : InterceptorBase, 
        IGrowlNotificationFinaliserInterceptor
    {
        /// <summary>
        /// The friendly id of a HealthCheck to attach this finaliser to. Use * to attach
        /// to all HealthChecks.
        /// </summary>
        public string Check { get; set; }

        /// <summary>
        /// This method should be implemented to inject your custom logic to 
        /// inspect the result and then set the Growl Notification properties.
        /// </summary>
        /// <param name="result">The HealthCheck result</param>
        /// <param name="notification">The Growl Notification - it will have default values for
        /// priority (normal) and message text as set by the <see cref="GrowlHealthCheckResultPublisher"/></param>
        public abstract void Finalise(HealthCheckResult result, Notification notification);

        /// <summary>
        /// This attaches this interceptor to the "Finalise" method being invoked on the 
        /// Growl Publisher. 
        /// </summary>
        protected GrowlNotificationInterceptorBase() : base("Finalise")
        {
        }

        protected override void HandleIntercept(IInvocation invocation)
        {
            var result = (HealthCheckResult)invocation.GetArgumentValue(0);
            var notification = (Notification)invocation.GetArgumentValue(1);

            if (Applies(result))
            {
                var prePriority = notification.Priority;
                Finalise(result, notification);

                if (notification.Priority != prePriority)
                    Logger.Debug("Finaliser '{0}' has adjusted the priority from {1} to {2}", GetType().Name, 
                        prePriority,
                        notification.Priority);
            }

            invocation.Proceed();
        }

        /// <summary>
        /// This will compare the finaliser configuration to the HealthCheck that generated this
        /// result to see if this finaliser should be applied to this result.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool Applies(HealthCheckResult result)
        {
            if (result == null)
                return false;

            return ((string.Compare(result.Check.Identity.Name, Check, true) == 0) ||
                    (string.Compare(Check, "*", true) == 0));
        }
    }
}