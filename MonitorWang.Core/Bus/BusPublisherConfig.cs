using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Bus
{
    public class BusConfig : PluginConfigBase
    {
        public string ErrorQueue { get; set; }
        public string InputQueue { get; set; }
    }

    public class BusPublisherConfig : BusConfig
    {
        public string HttpGatewayUri { get; set; }
        public string OutputQueue { get; set; }
    }
}