using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Interfaces
{
    public interface IMessenger
    {
        IMessenger Publish<T>(T message) where T: class;
        IMessenger Publish(HealthCheckAgentStart message);
        IMessenger Publish(HealthCheckResult message);
        IMessenger Publish(HealthCheckData message);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Target type to consume the message</typeparam>
        /// <param name="consumer"></param>
        /// <returns></returns>
        IMessenger Subscribe<T>(T consumer) where T : class;
    }
}