using Castle.DynamicProxy;

namespace MonitorWang.Core.Interfaces.Castle
{
    public interface IPublisherFilter : IInterceptor
    {
        string Publisher { get; set; }
        string Check { get; set; }
    }
}