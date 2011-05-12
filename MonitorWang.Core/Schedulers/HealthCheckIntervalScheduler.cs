using System;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Schedulers
{
    public class HealthCheckIntervalSchedulerConfig : IntervalSchedulerConfig
    {
        
    }

    public class HealthCheckIntervalScheduler : IntervalSchedulerBase, IHealthCheckSchedulerPlugin
    {
        protected IHealthCheckPlugin myHealthCheck;
        protected PluginDescriptor myIdentity;

        public HealthCheckIntervalScheduler(IHealthCheckPlugin check,
            HealthCheckIntervalSchedulerConfig config) 
            : base(config)
        {
            myHealthCheck = check;
            myIdentity = new PluginDescriptor
                             {
                                 TypeId = check.Identity.TypeId,
                                 Description = check.Identity.Description,
                                 Name = check.Identity.Name,
                                 ScheduleDescription = string.Format("Every {0} Minutes", myInterval.TotalMinutes)
                             };
        }

        public override PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        /// <summary>
        /// Initialises the HealthCheck plugin
        /// </summary>
        public override void Initialise()
        {
            myHealthCheck.Initialise();
        }

        /// <summary>
        /// Executes the health check plugin
        /// </summary>
        protected override void Execute()
        {
            try
            {
                myHealthCheck.Execute();
            }
            catch (Exception ex)
            {
                var incidentCorrelationId = Guid.NewGuid();
                var msg = string.Format("MonitorWang Component Failure. IncidentId:={0}; Name:={1}; Details:={2}",
                    incidentCorrelationId,
                    myHealthCheck.Identity.Name,
                    ex);

                Logger.Error(msg);

                // Broadcast a failure message
                Messenger.Publish(new HealthCheckData
                                          {
                                              CriticalFailure = true,
                                              CriticalFailureDetails = new CriticalFailureDetails
                                                                           {
                                                                               Id = incidentCorrelationId
                                                                           },
                                              GeneratedOnUtc = DateTime.UtcNow,
                                              Identity = myHealthCheck.Identity
                                          });
            }            
        }
    }
}