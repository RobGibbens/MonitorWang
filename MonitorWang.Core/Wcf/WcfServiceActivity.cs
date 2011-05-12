using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Wcf
{
    public class WcfServiceHostConfig : ConfigBase
    {
        public string ServiceImplementation { get; set; }
        public string Uri { get; set; }
    }

    /// <summary>
    /// Provides a WCF servicehost suitable for hosting within a windows service
    /// </summary>
    public class WcfServiceHost : IActivityPlugin
    {
        protected readonly WcfServiceHostConfig myConfig;
        protected readonly Type myServiceImplementation;
        protected readonly PluginDescriptor myIdentity;
        
        protected ServiceHost myServiceHost;

        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="config"></param>
        public WcfServiceHost(WcfServiceHostConfig config)
        {
            myConfig = config;
            myServiceImplementation = Type.GetType(config.ServiceImplementation, true, true);
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("WcfServiceHost for ServiceType '{0}'", config.ServiceImplementation),
                                 Name = "WcfServiceHost",
                                 TypeId = new Guid("EB06520D-577F-4149-A728-C3953F7C112D")
                             };
        }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public void Start()
        {
            myServiceHost = new ServiceHost(myServiceImplementation, new Uri(myConfig.Uri));
            myServiceHost.AddServiceEndpoint(typeof (IMonitorWang),
                                             new BasicHttpBinding(), 
                                             string.Empty);

            // Add MEX to the Endpoint
            // http://msdn.microsoft.com/en-us/library/aa738489.aspx
            var smb = new ServiceMetadataBehavior
                          {
                              HttpGetEnabled = true,
                              MetadataExporter = {PolicyVersion = PolicyVersion.Policy15}
                          };
            myServiceHost.Description.Behaviors.Add(smb);
            myServiceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
              MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            myServiceHost.Open();

            Logger.Debug("\t\t{0} is listening on these endpoints...", Identity.Name);
            myServiceHost.Description.Endpoints.ToList().ForEach(
                t => Logger.Debug("\t\t{0}", t.ListenUri));
        }

        public void Stop()
        {
            switch (myServiceHost.State)
            {
                case CommunicationState.Opened:
                    // this is a blocking call...
                    myServiceHost.Close();
                    break;

                case CommunicationState.Faulted:
                    // ensures resources are released
                    myServiceHost.Abort();
                    break;
            }
        }

        /// <summary>
        /// Pauses the current task activity (gracefully)
        /// </summary>
        public void Pause()
        {
            
        }

        /// <summary>
        /// Continues task activity
        /// </summary>
        public void Continue()
        {
            
        }

        public Status Status { get; set; }

        public void Initialise()
        {
            
        }

        public bool Enabled
        {
            get { return myConfig.Enabled; }
            set { myConfig.Enabled = value; }
        }
    }
}