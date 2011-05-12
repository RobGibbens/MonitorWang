
using System.Collections;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Tests.Mocks
{
    public interface IMockMessenger : IMessenger
    {
        ArrayList Sent { get; set; }
        ArrayList Subscribers { get; set; }
    }

    public class MockMessenger : IMockMessenger
    {
        public ArrayList Sent { get; set; }
        public ArrayList Subscribers { get; set; }

        public MockMessenger()
        {
            Sent = new ArrayList();
            Subscribers = new ArrayList();
        }

        public IMessenger Publish<T>(T message) where T : class
        {
            Sent.Add(message);
            return this;
        }

        public IMessenger Publish(HealthCheckAgentStart message)
        {
            Sent.Add(message);
            return this;
        }

        public IMessenger Publish(HealthCheckResult message)
        {
            Sent.Add(message);
            return this;
        }

        public IMessenger Publish(HealthCheckData message)
        {
            Sent.Add(message);
            return this;
        }

        public IMessenger Subscribe<T>(T consumer) where T : class
        {
            Subscribers.Add(consumer);
            return this;
        }
    }
}