using Castle.DynamicProxy;
using MonitorWang.Core.Containers;
using MonitorWang.Core.Interfaces.Castle;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Interfaces.Magnum;

namespace MonitorWang.Core.Filters
{
    public abstract class ResultFilterBase : InterceptorBase, IPublisherFilter
    {
        public string Publisher { get; set; }
        public string Check { get; set; }

        protected ResultFilterBase()
            : base("Consume")
        {
        }

        protected abstract bool ShouldPublish(IHealthCheckResultPublisher publisher, 
            HealthCheckResult message);

        protected override void HandleIntercept(IInvocation invocation)
        {
            var message = invocation.GetArgumentValue(0) as HealthCheckResult;
            var publisher = invocation.InvocationTarget as IHealthCheckResultPublisher;

            if (Applies(publisher, message))
            {
                // run the filter...
                if (ShouldPublish(publisher, message))
                    invocation.Proceed();
            }
            else
            {
                invocation.Proceed();
            }
        }

        protected bool Applies(IHealthCheckResultPublisher publisher, HealthCheckResult message)
        {
            if ((message == null) || (publisher == null))
                return false;

            var publisherMatch = ((string.Compare(publisher.FriendlyId, Publisher, true) == 0) ||
                                  (string.Compare(Publisher, "*", true) == 0));
            var checkMatch = ((string.Compare(message.Check.Identity.Name, Check, true) == 0) ||
                    (string.Compare(Check, "*", true) == 0));
            return publisherMatch && checkMatch;
        }        
    }
}