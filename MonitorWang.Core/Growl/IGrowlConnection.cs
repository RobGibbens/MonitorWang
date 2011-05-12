using Growl.Connector;

namespace MonitorWang.Core.Growl
{
    public interface IGrowlConnection
    {
        GrowlConnector Connection { get; }
        GrowlConfiguration Config { get; }
    }
}