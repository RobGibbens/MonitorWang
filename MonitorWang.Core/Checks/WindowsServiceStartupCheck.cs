using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Checks
{
    public class WindowsServiceStartupCheckConfig : PluginConfigBase
    {
        /// <summary>
        /// List of windows services to check the startup type of. This can be
        /// the Display name or Short name of the service.
        /// </summary>
        public List<string> Services { get; set; }
        /// <summary>
        /// The server to monitor the windows services on
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// The Startup Type you expect these services to be in; values are
        /// <list type="bullet">
        /// <item><description>Auto</description></item>
        /// <item><description>Manual</description></item>
        /// <item><description>Disabled</description></item>
        /// </list>
        /// </summary>
        public string ExpectedStartupType { get; set; }

        /// <summary>
        /// Helper method to quickly validate the <see cref="ExpectedStartupType"/>
        /// configuration value is valid
        /// </summary>
        /// <returns></returns>
        public bool StartupTypeIsValid()
        {
            return ParameterValueIsInList(ExpectedStartupType, "auto", "manual", "disabled");
        }
    }

    /// <summary>
    /// This HealthCheck will monitor a list of windows services and checks that
    /// they are have the startup type specified in the configuration.
    /// It will only publish as HealthCheck result if the service does not have the 
    /// correct startup type. A HealthCheck result is published for EACH service that 
    /// fails the check.
    /// </summary>
    /// <remarks>This HealthCheck uses WMI to return information about the Windows Services,
    /// the .Net ServiceController wrapper does not return the Startup Type so we have to
    /// use WMI to do this</remarks>
    public class WindowsServiceStartupCheck : IHealthCheckPlugin
    {
        protected readonly WindowsServiceStartupCheckConfig myConfig;
        protected readonly PluginDescriptor myIdentity;
        protected readonly string myServer;
        protected readonly string myWmiNamespace;

        public WindowsServiceStartupCheck(WindowsServiceStartupCheckConfig config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
            {
                Description = string.Format("Ensures all specified windows services have the expected startup type '{0}'", myConfig.ExpectedStartupType),
                TypeId = new Guid("F3EA664E-B753-471f-87F4-C15D0395B11A"),
                Name = myConfig.FriendlyId
            };

            myServer = string.IsNullOrEmpty(myConfig.Server) ? "." : myConfig.Server;
            myWmiNamespace = string.Format(@"\\{0}\root\cimv2", myServer);
        }

        public Status Status { get; set; }

        public void Initialise()
        {
            Logger.Debug("Initialising WindowsServiceStartup check for...");

            if (!myConfig.StartupTypeIsValid())
                throw new FormatException(
                    string.Format("Value '{0}' for configuration property 'ExpectedStartupType' is not valid",
                                  myConfig.ExpectedStartupType));

            myConfig.Services.ForEach(service => Logger.Debug("\t{0}", service));
            Logger.Debug("\tComplete, monitoring services for expected startup type '{0}' on Server '{1}'",
                myConfig.ExpectedStartupType, myServer);
        }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public void Execute()
        {
            Logger.Debug("WindowsServiceStartupCheck is checking service startup types...");

            // use the service/current identity to query local or remote
            var wmiScope = new ManagementScope(myWmiNamespace, new ConnectionOptions
                      {
                          Impersonation = ImpersonationLevel.Impersonate
                      });

            // set up the query and execute it
            var wmiQuery = new ObjectQuery("Select * from Win32_Service");
            var wmiSearcher = new ManagementObjectSearcher(wmiScope, wmiQuery);
            var wmiResults = wmiSearcher.Get();
            var results = wmiResults.Cast<ManagementObject>();
            var services = from service in results
                           select new
                                      {
                                          Name = service["Name"].ToString(),
                                          DisplayName = service["DisplayName"].ToString(),
                                          StartMode = service["StartMode"].ToString()
                                      };

            // find the services we are interested in that do not have the
            // startup type (startmode) we expect
            var faultedServices = (from service in services
                                   from name in myConfig.Services
                                   where MatchName(name, service.Name, service.DisplayName)
                                          && (string.Compare(myConfig.ExpectedStartupType, service.StartMode, true) != 0)
                                   select service).ToList();

            faultedServices.ForEach(fs =>
                                             {
                                                 var result = new HealthCheckData
                                                                  {
                                                                      Identity = Identity,
                                                                      Info = string.Format("{0} should be {1} but is {2}",
                                                                      fs.DisplayName, myConfig.ExpectedStartupType, fs.StartMode),
                                                                      Result = false
                                                                  };
                                                 Messenger.Publish(result);
                                             });

            // invalid/unknown service names...
            var invalidServices = myConfig.Services.Where(configService =>
                !services.Any(svc => MatchName(configService, svc.Name, svc.DisplayName)));

            if (invalidServices.Count() > 0)
                throw new InvalidOperationException(string.Format("These services do not exist: {0}",
                    string.Join(",", invalidServices.ToArray())));
        }

        private static bool MatchName(string requiredService, string name, string displayName)
        {
            return (string.Compare(requiredService, name, true) == 0) ||
                   (string.Compare(requiredService, displayName, true) == 0);
        }
    }
}