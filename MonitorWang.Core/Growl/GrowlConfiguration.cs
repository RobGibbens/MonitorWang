using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Growl
{
    public class GrowlConfiguration : PluginConfigBase
    {
        public string IconFile { get; set; }
        public string AppId { get; set; }
        public string NotificationId { get; set; }
        public string NotificationTitle { get; set; }
        public string Hostname { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

        public GrowlConfiguration()
        {
            // default GNTP port
            Port = 23053;
        }
    }
}