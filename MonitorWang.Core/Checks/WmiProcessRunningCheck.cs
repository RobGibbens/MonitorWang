using System;
using System.Linq;
using System.Management;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Checks
{
    public class WmiProcessRunningCheckConfig : PluginConfigBase
    {
        public string RemoteUser { get; set; }
        public string RemotePwd { get; set; } 
        public string RemoteMachineId { get; set; } 
        public string ProcessName { get; set; }
    }

    public class WmiProcessRunningCheck : IHealthCheckPlugin
    {
        protected readonly PluginDescriptor myIdentity;
        protected readonly string myWmiNamespace;
        protected readonly WmiProcessRunningCheckConfig myConfig;

        public WmiProcessRunningCheck(WmiProcessRunningCheckConfig config)
        {
            myConfig = config;
            myWmiNamespace = string.Format(@"\\{0}\root\cimv2", config.RemoteMachineId);

            myIdentity = new PluginDescriptor
            {
                Description =
                    string.Format("Checks for the existance of process '{0}' on {1}", myConfig.ProcessName,
                                  myConfig.RemoteMachineId),
                TypeId = new Guid("46D4374C-C65D-442e-9B93-AF50BB8C045C"),
                Name = myConfig.FriendlyId
            };
        }

        public Status Status { get; set; }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public void Execute()
        {
            ManagementScope wmiScope;

            Logger.Debug("Querying wmi namespace {0}...", myWmiNamespace);
            if (!string.IsNullOrEmpty(myConfig.RemoteUser) && !string.IsNullOrEmpty(myConfig.RemotePwd))
            {
                wmiScope = new ManagementScope(myWmiNamespace, new ConnectionOptions
                {
                    Username = myConfig.RemoteUser,
                    Password = myConfig.RemotePwd
                });
            }
            else
            {
                wmiScope = new ManagementScope(myWmiNamespace);
            }

            // set up the query and execute it
            var wmiQuery = new ObjectQuery("Select * from Win32_Process");
            var wmiSearcher = new ManagementObjectSearcher(wmiScope, wmiQuery);
            var wmiResults = wmiSearcher.Get();
            var processes = wmiResults.Cast<ManagementObject>();

            var matches = from process in processes
                          where
                              (string.Compare(process["Name"].ToString(), myConfig.ProcessName,
                                              StringComparison.InvariantCultureIgnoreCase) == 0)
                          select process;

            var msg = new HealthCheckData
                          {
                              Identity = Identity,
                              Info = string.Format("There are {0} instances of process '{1}' on {2}",
                                                   matches.Count(),
                                                   myConfig.ProcessName,
                                                   myConfig.RemoteMachineId),
                              Result = (matches.Count() > 0),
                              ResultCount = matches.Count()
                          };

            Messenger.Publish(msg);            
        }

        public void Initialise()
        {
            
        }
    }
}