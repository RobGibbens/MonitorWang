using Magnum.Pipeline;
using Magnum.Pipeline.Segments;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core
{
    /// <summary>
    /// This implementation uses the Magnum Event Aggregator/Bus for
    /// decoupled intercomponent communication
    /// </summary>
    public class Messenger
    {
        private static IMessenger myInstance;

        public static void Initialise(IMessenger instance)
        {
            myInstance = instance; 
        }

        public static IMessenger Publish<T>(T message) where T : class
        {
            return myInstance.Publish(message);
        }

        public static IMessenger Publish(HealthCheckAgentStart message)
        {
            return myInstance.Publish(message);           
        }

        public static IMessenger Publish(HealthCheckResult message)
        {
            return myInstance.Publish(message);
        }

        public static IMessenger Publish(HealthCheckData message)
        {
            return myInstance.Publish(message);
        }

        public static IMessenger Subscribe<T>(T consumer) where T: class
        {
            return myInstance.Subscribe(consumer);
        }
    }

    /// <summary>
    /// This implementation uses the Magnum Event Aggregator/Bus for
    /// decoupled intercomponent communication
    /// </summary>
    public class MagnumMessenger : IMessenger
    {
        private static readonly InputSegment myMessageBus;
        private static readonly ISubscriptionScope mySubscriptionScope;

        static MagnumMessenger()
        {
            myMessageBus = PipeSegment.Input(PipeSegment.End());
            mySubscriptionScope = myMessageBus.NewSubscriptionScope();
        }

        public IMessenger Publish<T>(T message) where T : class
        {
            myMessageBus.Send(message);
            return this;
        }

        IMessenger IMessenger.Publish(HealthCheckAgentStart message)
        {
            myMessageBus.Send(message);
            return this;
        }

        IMessenger IMessenger.Publish(HealthCheckResult message)
        {
            myMessageBus.Send(message);
            return this;
        }

        IMessenger IMessenger.Publish(HealthCheckData message)
        {
            myMessageBus.Send(message);
            return this;
        }

        public IMessenger Subscribe<T>(T consumer) where T : class
        {
            mySubscriptionScope.Subscribe(consumer);
            return this;
        }
    }
}