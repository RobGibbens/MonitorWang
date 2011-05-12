using Magnum.Pipeline;

namespace MonitorWang.Core.Interfaces.Magnum
{
    public interface IPublisherPlugin<T> : IPlugin, ICanBeSwitchedOff, IConsumer<T>
        where T: class
    {
        string FriendlyId { get; set; }
    }
}