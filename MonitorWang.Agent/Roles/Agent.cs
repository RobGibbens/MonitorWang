using System;
using System.Linq;
using Magnum.Pipeline;
using MonitorWang.Core;
using MonitorWang.Core.Hosts;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Core.Interfaces.Magnum;

namespace MonitorWang.Agent.Roles
{
    public class Agent : PluginHostBase, IRolePlugin, IConsumer<HealthCheckData>
    {
        protected readonly ILoader<IHealthCheckSessionPublisher> mySessionPublisherLoader;
        protected readonly ILoader<IHealthCheckResultPublisher> myResultPublisherLoader;
        protected readonly ILoader<IHealthCheckSchedulerPlugin> myChecksLoader;
        protected readonly ILoader<IActivityPlugin> myActivitiesLoader;
        protected PluginDescriptor myIdentity;

        protected AgentInfo myAgentInfo;

        public Agent(AgentConfiguration config,
            ILoader<IHealthCheckSessionPublisher> sessionPublisherLoader,
            ILoader<IHealthCheckResultPublisher> resultPublisherLoader,
            ILoader<IHealthCheckSchedulerPlugin> checksLoader,
            ILoader<IActivityPlugin> activitiesLoader)
        {
            mySessionPublisherLoader = sessionPublisherLoader;
            myResultPublisherLoader = resultPublisherLoader;
            myChecksLoader = checksLoader;
            myActivitiesLoader = activitiesLoader;
            myAgentInfo = BuildAgentInfo(config);

            myIdentity = new PluginDescriptor
                             {
                                 Description = "Agent description [TODO]",
                                 Name = "Agent",
                                 TypeId = new Guid("649D0AAC-3AA0-4457-B82D-F834EA324CFA")
                             };
        }

        private static AgentInfo BuildAgentInfo(AgentConfiguration config)
        {
            return new AgentInfo
                       {
                           SiteId = config.SiteId,
                           AgentId = Environment.MachineName
                       };
        }

        public override PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public override void Start()
        {
            // start the load process....
            var sessionInfo = new HealthCheckAgentStart
            {
                DiscoveryStarted = DateTime.UtcNow,
                Agent = myAgentInfo
            };


            // load publishers
            IHealthCheckSessionPublisher[] sessionPublishers;
            mySessionPublisherLoader.Load(out sessionPublishers, p =>
                                                                     {
                                                                         if (!p.Status.IsHealthy())
                                                                         {
                                                                             Logger.Debug("*** Session Publisher '{0}' reporting 'unhealthy', disabling it ***", 
                                                                                 p.FriendlyId);
                                                                             return;
                                                                         }

                                                                         Messenger.Subscribe(p);
                                                                         Logger.Debug("Loaded Session Publisher '{0}'",
                                                                                      p.GetType().Name);
                                                                     });
            IHealthCheckResultPublisher[] resultPublishers;
            myResultPublisherLoader.Load(out resultPublishers, p =>
                                                                   {
                                                                       if (!p.Status.IsHealthy())
                                                                       {
                                                                           Logger.Debug("*** Result Publisher '{0}' reporting 'unhealthy', disabling it ***",
                                                                               p.FriendlyId);
                                                                           return;
                                                                       }

                                                                       Messenger.Subscribe(p);
                                                                       Logger.Debug("Loaded Result Publisher '{0}'",
                                                                                    p.GetType().Name);
                                                                   });
            
            // load activities...
            IActivityPlugin[] activities;
            if (myActivitiesLoader.Load(out activities))
                activities.ToList().ForEach(a =>
                {
                    if (!a.Status.IsHealthy())
                    {
                        Logger.Debug("*** Activity '{0}' reporting 'unhealthy', skipping it ***", a.Identity.Name);
                        return;
                    }

                    myPlugins.Add(a);
                    Logger.Debug("Loaded Activity '{0}'", a.GetType().Name);
                });

            // load health checks...
            IHealthCheckSchedulerPlugin[] healthChecks;
            myChecksLoader.Load(out healthChecks);
            healthChecks.ToList().ForEach(h =>
                                              {
                                                  if (!h.Status.IsHealthy())
                                                  {
                                                      Logger.Debug("*** HealthCheck '{0}' reporting 'unhealthy', skipping it ***", h.Identity.Name);
                                                      return;
                                                  }

                                                  myPlugins.Add(h);
                                                  Logger.Debug("Loaded HealthCheck '{0}'", h.Identity.Name);
                                              });

            // extract check info, attach and publish it to a session message
            sessionInfo.DiscoveryCompleted = DateTime.UtcNow;
            sessionInfo.Checks = (from healthCheck in healthChecks
                                  where healthCheck.Status.IsHealthy()
                                  select healthCheck.Identity).ToList();
            sessionInfo.UnhealthyChecks = (from healthCheck in healthChecks
                                  where !healthCheck.Status.IsHealthy()
                                  select healthCheck.Identity).ToList();
            sessionInfo.Activities = (from activity in activities
                                      where activity.Status.IsHealthy()
                                      select activity.Identity).ToList();
            sessionInfo.UnhealthyActivities = (from activity in activities
                                      where !activity.Status.IsHealthy()
                                      select activity.Identity).ToList();

            Messenger.Publish(sessionInfo);

            // listen for result messages being published
            // by the checks & activities - route to the
            // publisher-hub so it can decide how to handle them
            Messenger.Subscribe(this);
            // finally start the checks & activities
            base.Start();
        }

        public void Consume(HealthCheckData message)
        {
            // wrap the data with the agent information
            // to produce a "result"
            var result = new HealthCheckResult
            {
                Agent = myAgentInfo,
                Check = message
            };

            Messenger.Publish(result);
        }
    }
}