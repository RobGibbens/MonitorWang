using System.Collections.Generic;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Interfaces.Magnum;

namespace MonitorWang.Core.Filters
{
    public class EventTypeFilter : ResultFilterBase
    {
        public List<string> PublishTheseTypes { get; set; }
        public List<string> IgnoreTheseTypes { get; set; }

        protected override bool ShouldPublish(IHealthCheckResultPublisher publisher, HealthCheckResult message)
        {
            if (IgnoreTheseTypes != null && IgnoreTheseTypes.Count > 0)
            {
                // don't publish if match
                if (IgnoreTheseTypes.Exists(ignoreType => string.Compare(ignoreType, message.EventType) == 0))
                    return false;
                // otherwise check the publish list next....
            } 
            
            if (PublishTheseTypes != null && PublishTheseTypes.Count > 0)
            {
                // publish only if match
                return PublishTheseTypes.Exists(publishType => string.Compare(publishType, message.EventType) == 0);
            }

            // publish everything else...
            return true;
        }
    }
}