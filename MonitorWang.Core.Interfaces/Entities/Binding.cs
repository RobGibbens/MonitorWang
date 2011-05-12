namespace MonitorWang.Core.Interfaces.Entities
{
    public class Binding
    {
        public IHealthCheckPlugin Check { get; set; }
        public IHealthCheckSchedulerPlugin Scheduler { get; set; }
    }
}