using MonitorWang.Core.Checks;
using MonitorWang.Tests.Bdd;

namespace MonitorWang.Tests.Checks
{   
    public class WindowServiceStateDomain : HealthCheckDomain
    {
        public void TheCheckComponent(WindowsServiceStateCheckConfig config)
        {
            HealthCheck = new WindowsServiceStateCheck(config);
            HealthCheck.Initialise();
        }
    }
}