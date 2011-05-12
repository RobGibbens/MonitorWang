using System;
using System.Collections.Generic;
using System.ServiceProcess;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Checks
{
    public class WindowsServiceStateCheckConfig : PluginConfigBase
    {
        /// <summary>
        /// List of windows services to check the state of. This can be
        /// the Display name or Short name of the service.
        /// </summary>
        public List<string> Services { get; set; }
        /// <summary>
        /// The server to monitor the windows services on
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// The state you expect these services to be in; values are
        /// <list type="bullet">
        /// <item><description>Stopped</description></item>
        /// <item><description>Running</description></item>
        /// </list>
        /// </summary>
        public string ExpectedState { get; set; }

        /// <summary>
        /// Helper method to quickly validate the <see cref="ExpectedState"/>
        /// configuration value is valid
        /// </summary>
        /// <returns></returns>
        public bool StateIsValid()
        {
            return ParameterValueIsInList(ExpectedState, "running", "stopped");
        }
    }

    /// <summary>
    /// This HealthCheck will monitor a list of windows services and checks that
    /// they are in the state (Running or Stopped) specified in the configuration.
    /// It will only publish as HealthCheck result if the service is not in the state
    /// expected. A HealthCheck result is published for EACH service that fails the check.
    /// </summary>
    public class WindowsServiceStateCheck : IHealthCheckPlugin
    {
        protected readonly WindowsServiceStateCheckConfig myConfig;
        protected readonly PluginDescriptor myIdentity;
        protected readonly string myServer;
        protected ServiceControllerStatus myExpectedState;

        public WindowsServiceStateCheck(WindowsServiceStateCheckConfig config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
            {
                Description = string.Format("Ensures all specified windows services are in the expected state {0}", myConfig.ExpectedState),
                TypeId = new Guid("BEFDC111-ED9A-4aee-A3B9-4251BF9F833A"),
                Name = myConfig.FriendlyId
            };

            myServer = string.IsNullOrEmpty(myConfig.Server) ? "." : myConfig.Server;
        }

        public Status Status { get; set;}

        public void Initialise()
        {
            Logger.Debug("Initialising WindowsServiceState check for...");
            if (!myConfig.StateIsValid())
                throw new FormatException(
                    string.Format("Value '{0}' for configuration property 'ExpectedState' is not valid",
                                  myConfig.ExpectedState));
            // save this for use in the check later
            myExpectedState = (ServiceControllerStatus) Enum.Parse(typeof (ServiceControllerStatus), myConfig.ExpectedState, true);

            Logger.Debug("\tComplete, monitoring services for expected state '{0}' on Server '{1}'", 
                myConfig.ExpectedState, myServer);
        }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public void Execute()
        {
            Logger.Debug("WindowsServiceStateCheck is checking service states...");
            var failures = new List<string>();

            myConfig.Services.ForEach(serviceName =>
                                             {
                                                 try
                                                 {
                                                     var sc = new ServiceController(serviceName, myServer);

                                                     if (sc.Status == myExpectedState)
                                                         return;

                                                     var result = new HealthCheckData
                                                     {
                                                         Identity = Identity,
                                                         Info = string.Format("{0} should be {1} but is {2}",
                                                         sc.DisplayName, myConfig.ExpectedState, sc.Status),
                                                         Result = false
                                                     };
                                                     Messenger.Publish(result);
                                                 }
                                                 catch (InvalidOperationException)
                                                 {
                                                     failures.Add(serviceName);
                                                 }
                                             });
            if (failures.Count > 0)
            {
                throw new InvalidOperationException(string.Format("These services do not exist: {0}",
                    string.Join(",", failures.ToArray())));
            }
        }
    }
}