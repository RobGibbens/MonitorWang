using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Loaders
{
    public static class PluginExtensions
    {
        public static IEnumerable<IPlugin> InitialisePlugins(this IEnumerable<IPlugin> plugins)
        {
            plugins.ForEach(c =>
            {
                Logger.Event error;
                if (!c.SafeInitialise(out error))
                {
                    Logger.Error(error);
                }
            });

            return plugins;
        }

        public static IEnumerable<IPlugin> ExcludeUnhealthyPlugins(this IEnumerable<IPlugin> plugins)
        {
            return from plugin in plugins
                   where (plugin.Status == null) || (string.Compare(plugin.Status.State, "Success") == 0)
                   select plugin;
        }

        /// <summary>
        /// This will attempt to initialise the component but will trap
        /// any exeception and set the plugin status accordingly if it fails
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool SafeInitialise(this IPlugin plugin, out Logger.Event error)
        {
            error = null;

            try
            {
                Logger.Debug("Initialising plugin '{0}'...", plugin.GetType().Name);
                plugin.Status = Status.For("Initialisation").StateIsPending();
                plugin.Initialise();
                plugin.Status.StateIsSuccess();
            }
            catch (Exception ex)
            {
                error = Logger.Event.During("Initialisation")
                    .Encountered(ex);
                plugin.Status = error.Context;
            }

            return (error == null);
        }
    }
}