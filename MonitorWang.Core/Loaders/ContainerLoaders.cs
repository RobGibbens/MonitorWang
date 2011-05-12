using System;
using System.Linq;
using MonitorWang.Core.Interfaces;
using Castle.Core;

namespace MonitorWang.Core.Loaders
{    
    /// <summary>
    /// This will load a set of components from the IoC container
    /// </summary>
    public class ContainerLoader<TI> : ILoader<TI>
    {
        /// <summary>
        /// Helper to directly use this components
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool Resolve(out TI[] components)
        {
            return new ContainerLoader<TI>().Load(out components);
        }

        /// <summary>
        /// Helper to directly use this components
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool Resolve(out TI[] components, Action<TI> action)
        {
            return new ContainerLoader<TI>().Load(out components, action);
        }

        public bool Load(out TI[] components)
        {
            var candidates = (from plugin in Container.ResolveAll<TI>()
                              select plugin).ToList();

            components = candidates.ToArray();
            return (components.Length > 0);
        }

        public bool Load(out TI[] components, Action<TI> action)
        {
            var result = Load(out components);
            components.ForEach(action);
            return result;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ContainerPluginLoader<TI> : ILoader<TI>
        where TI : IPlugin, ICanBeSwitchedOff
    {
        /// <summary>
        /// Helper to directly use this components
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool Resolve(out TI[] components)
        {
            return new ContainerPluginLoader<TI>().Load(out components);
        }

        /// <summary>
        /// Helper to directly use this components
        /// </summary>
        /// <param name="components"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool Resolve(out TI[] components, Action<TI> action)
        {
            return new ContainerPluginLoader<TI>().Load(out components, action);
        }

        public bool Load(out TI[] components)
        {
            var candidates = (from plugin in Container.ResolveAll<TI>()
                              where plugin.Enabled
                              select plugin).ToList();

            var plugins = candidates.Cast<IPlugin>().InitialisePlugins();
            components = plugins.Cast<TI>().ToArray();
            return (components.Length > 0);
        }

        public bool Load(out TI[] components, Action<TI> action)
        {
            var result = Load(out components);
            components.ForEach(action);
            return result;
        }
    }
}