using System.IO;
using Growl.Connector;

namespace MonitorWang.Core.Growl
{
    public class GrowlConnection : IGrowlConnection
    {
        public const string DEFAULT_ICON = @"growl\growl.monitor.png";

        protected readonly GrowlConfiguration myConfig;
        protected readonly GrowlConnector myConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public GrowlConnection(GrowlConfiguration config)
        {
            myConfig = config;

            // create the growl connection and register it            
            if (!string.IsNullOrEmpty(config.Password))
            {
                myConnector = !string.IsNullOrEmpty(config.Hostname)
                                  ? new GrowlConnector(config.Password, config.Hostname, config.Port)
                                  : new GrowlConnector(config.Password);
            }
            else
                myConnector = new GrowlConnector();

            var application = new Application(config.AppId);

            // use the defaul icon if no override set
            if (string.IsNullOrEmpty(config.IconFile))
                config.IconFile = DEFAULT_ICON;
            var icon = new SmartLocation(config.IconFile);
            if (File.Exists(icon.Location))
                application.Icon = icon.Location;

            var healthCheck = new NotificationType(config.NotificationId, config.NotificationTitle);
            myConnector.Register(application,
                                 new[]
                                         {
                                             healthCheck
                                         });            

        }

        public GrowlConnector Connection
        {
            get { return myConnector; }
        }

        public GrowlConfiguration Config
        {
            get { return myConfig; }
        }
    }
}