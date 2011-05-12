using System;
using System.Collections.Generic;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Hosts
{
    public abstract class PluginHostBase : IServicePlugin
    {
        protected List<IServicePlugin> myPlugins = new List<IServicePlugin>();

        public abstract PluginDescriptor Identity { get; }

        public Status Status { get; set; }

        public void Initialise()
        {
            Logger.Debug("Initialising plugins...");
            myPlugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Initialise();
            });
        }

        public virtual void Start()
        {
            Logger.Debug("Starting plugins...");
            myPlugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Start();
            });
        }

        public virtual void Stop()
        {
            Logger.Debug("Stopping plugins...");
            myPlugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Stop();
            });
        }

        public virtual void Pause()
        {
            Logger.Debug("Pausing plugins...");
            myPlugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Pause();
            });
        }

        public virtual void Continue()
        {
            Logger.Debug("Continuing plugins...");
            myPlugins.ForEach(p =>
            {
                Logger.Debug("\t{0}", p.GetType().Name);
                p.Continue();
            });
        }
    }
}