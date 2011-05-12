using MonitorWang.Core.Checks;
using MonitorWang.Tests.Bdd;

namespace MonitorWang.Tests.Checks
{
    public class WindowServiceStartupDomain : HealthCheckDomain
    {
        public void TheCheckComponent(WindowsServiceStartupCheckConfig config)
        {
            HealthCheck = new WindowsServiceStartupCheck(config);
            HealthCheck.Initialise();
        }
    }
}