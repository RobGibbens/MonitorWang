using System.ServiceModel;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Wcf
{
    [ServiceContract(Namespace = "http://monitorwang.iagileservices.com")]
    public interface IMonitorWang
    {
        [OperationContract(IsOneWay = true)]
        void CaptureAgentStart(HealthCheckAgentStart session);

        [OperationContract(IsOneWay = true)]
        void CaptureResult(HealthCheckResult result);
        
    }
}