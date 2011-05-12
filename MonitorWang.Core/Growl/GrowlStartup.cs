using System;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Growl
{
    public class GrowlStartup : IStartupPlugin, ICanBeSwitchedOff
    {
        protected readonly GrowlConfiguration myConfig;
        protected readonly PluginDescriptor myIdentity;

        public GrowlStartup(GrowlConfiguration config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("Initalises a Growl connection"),
                                 TypeId = new Guid("D040058E-8E88-49d2-B9E1-4794CE92702F"),
                                 Name = "GrowlConnector"
                             };
        }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }


        public bool Enabled
        {
            get { return myConfig.Enabled; }
            set { myConfig.Enabled = value; }
        }

        public Status Status { get; set; }

        public void Initialise()
        {
            if (!Enabled)
                return;

            Container.RegisterAsSingleton<IGrowlConnection>(typeof (GrowlConnection));
        }
    }
}