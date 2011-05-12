using System;
using System.Linq;
using System.Text;
using Growl.Connector;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Interfaces.Magnum;
using MonitorWang.Core.Publishers;

namespace MonitorWang.Core.Growl
{
    public class GrowlHealthCheckSessionPublisher : PublisherBase, IHealthCheckSessionPublisher
    {
        protected readonly GrowlConfiguration myConfig;
        protected readonly IGrowlConnection myGrowler;

        public GrowlHealthCheckSessionPublisher(GrowlConfiguration config,
            IGrowlConnection connection)
        {
            myConfig = config;
            myGrowler = connection;

            Enabled = config.Enabled;
            FriendlyId = config.FriendlyId;            
        }

        public void Publish(HealthCheckAgentStart message)
        {
            var isSticky = false;
            var textBuilder = new StringBuilder(string.Format("Agent on {0} started\n", message.Agent.AgentId));

            if ((message.Activities != null) && (message.Activities.Count > 0))
            {
                textBuilder.AppendFormat("{0} Activities loaded...\n", message.Activities.Count);
                message.Activities.ForEach(c => textBuilder.AppendFormat("{0}\n", c.Name));
            }
            if ((message.UnhealthyActivities != null) && (message.UnhealthyActivities.Count > 0))
            {
                textBuilder.Append("*** ATTENTION ***\n");
                textBuilder.AppendFormat("{0} Unheathly Activities...\n", message.UnhealthyActivities.Count);
                message.UnhealthyActivities.ForEach(c => textBuilder.AppendFormat("{0}\n", c.Name));
                isSticky = true;
            }
            if ((message.Checks != null) && (message.Checks.Count > 0))
            {
                textBuilder.AppendFormat("{0} Checks loaded...\n", message.Checks.Count);
                message.Checks.ForEach(c => textBuilder.AppendFormat("{0}\n", c.Name));
            }
            if ((message.UnhealthyChecks != null) && (message.UnhealthyChecks.Count > 0))
            {
                textBuilder.Append("*** ATTENTION ***\n");
                textBuilder.AppendFormat("{0} Unheathly Checks...\n", message.UnhealthyChecks.Count);
                message.UnhealthyChecks.ForEach(c => textBuilder.AppendFormat("{0}\n", c.Name));
                isSticky = true;
            }

            // remove CR from text as Growl doesn't like these...
            textBuilder.Replace("\r", string.Empty);

            var notification = new Notification(myGrowler.Config.AppId,
                                                myGrowler.Config.NotificationId, null,
                                                string.Format("{0} HealthCheck Agent",
                                                              message.Agent.SiteId),
                                                textBuilder.ToString())
                                   {
                                       Sticky = isSticky,
                                       Priority = isSticky ? Priority.Emergency : Priority.Normal
                                   };

            myGrowler.Connection.Notify(notification);
        }

        public void Consume(HealthCheckAgentStart message)
        {
            Publish(message);
        }
    }

    public class GrowlHealthCheckResultPublisher : PublisherBase, IHealthCheckResultPublisher
    {
        protected readonly GrowlConfiguration myConfig;
        protected readonly IGrowlConnection myGrowl;
        protected readonly IGrowlNotificationFinaliser myNotificationFinaliser;

        public GrowlHealthCheckResultPublisher(GrowlConfiguration config, 
            IGrowlConnection connection,
            IGrowlNotificationFinaliser finaliser)
        {
            myConfig = config;
            myGrowl = connection;
            myNotificationFinaliser = finaliser;

            Enabled = config.Enabled;
            FriendlyId = config.FriendlyId;
        }

        public void Publish(HealthCheckResult message)        
        {
            string resultLine1;
            var criticalFailure = false;

            if (!message.Check.Result.HasValue)
                resultLine1 = "Result is Unknown!";
            else if (message.Check.Result.Value)
                resultLine1 = "Passed!";
            else
                resultLine1 = "** Failed **";

            string resultLine2;
            string resultLine3 = null;
            string resultLine4 = null;

            if (message.Check.CriticalFailure)
            {
                resultLine2 = "*** CRITICAL FAILURE ***";
                resultLine3 = string.Format("MonitorWang Log Ref:={0}", message.Check.CriticalFailureDetails.Id);
                resultLine4 = message.Check.Info;
                criticalFailure = true;
            }
            else
            {
                resultLine2 = message.Check.Info;
            }

            var textBuilder = new StringBuilder(string.Format("{0}\n{1}\n",message.Check.Identity.Name,
                DateTime.Now));
            textBuilder.AppendIf(!string.IsNullOrEmpty(resultLine1), "{0}\n", resultLine1);
            textBuilder.AppendIf(!string.IsNullOrEmpty(resultLine2), "{0}\n", resultLine2);
            textBuilder.AppendIf(!string.IsNullOrEmpty(resultLine3), "{0}\n", resultLine3);
            textBuilder.AppendIf(!string.IsNullOrEmpty(resultLine4), "{0}\n", resultLine4);

            textBuilder.AppendIf(message.Check.ResultCount.HasValue, "Count:={0}\n",
                message.Check.ResultCount.GetValueOrDefault());

            if (message.Check.Properties != null)
            {
                message.Check.Properties.ToList().ForEach(
                    p => textBuilder.AppendFormat("{0}:={1}\n", p.Key, p.Value));
            }

            // remove CR from text as Growl doesn't like these...
            textBuilder.Replace("\r", string.Empty);

            var notification = new Notification(myGrowl.Config.AppId,
                                                myGrowl.Config.NotificationId, null,
                                                string.Format("{0} HealthCheck Result", message.Agent.SiteId),
                                                textBuilder.ToString())
                                   {
                                       Priority = criticalFailure ? Priority.Emergency : Priority.Normal,
                                       Sticky = criticalFailure
                                   };
            myNotificationFinaliser.Finalise(message, notification);
            myGrowl.Connection.Notify(notification);
        }

        public void Consume(HealthCheckResult message)
        {
            Publish(message);
        }
    }

    public class DefaultGrowlNotificationFinaliser : IGrowlNotificationFinaliser
    {
        public void Finalise(HealthCheckResult result, Notification notification)
        {
            // do nothing (provides a method hook for interception)
        }
    }
}