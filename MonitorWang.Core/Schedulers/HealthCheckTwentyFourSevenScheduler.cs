using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;
using System.Linq;

namespace MonitorWang.Core.Schedulers
{
    public class HealthCheckTwentyFourSevenSchedulerConfig : TwentyFourSevenTimerConfig
    {
        public int IntervalInSeconds { get; set; }

        public HealthCheckTwentyFourSevenSchedulerConfig()
        {
            IntervalInSeconds = 60;
        }
    }

    public class HealthCheckTwentyFourSevenScheduler : HealthCheckIntervalScheduler
    {
        protected TwentyFourSevenTimer myTimer;

        public HealthCheckTwentyFourSevenScheduler(IHealthCheckPlugin check,
            HealthCheckTwentyFourSevenSchedulerConfig config) 
            : base(check, new HealthCheckIntervalSchedulerConfig
                              {
                                  IntervalInSeconds = config.IntervalInSeconds
                              })
        {
            myIdentity = new PluginDescriptor
                             {
                                 TypeId = check.Identity.TypeId,
                                 Description = check.Identity.Description,
                                 Name = check.Identity.Name,
                                 ScheduleDescription = "24/7 Scheduler"
                             };

            myTimer = new TwentyFourSevenTimer(config);
        }

        /// <summary>
        /// Executes the health check plugin
        /// </summary>
        protected override void Execute()
        {
            if (myTimer.Triggered().Count() == 0)
                return;

            base.Execute();
        }
    }
}