using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Interfaces.Magnum;

namespace MonitorWang.Core.Filters
{
    public class SuccessFilter : ResultFilterBase
    {
        public bool PublishSuccess { get; set; }
        public bool PublishFailure { get; set; }

        protected override bool ShouldPublish(IHealthCheckResultPublisher publisher, HealthCheckResult message)
        {
            if (PublishFailure && !message.Check.Result.GetValueOrDefault(false))
                return true;
            if (PublishSuccess && message.Check.Result.GetValueOrDefault(false))
                return true;
            return false;
        }
    }
}