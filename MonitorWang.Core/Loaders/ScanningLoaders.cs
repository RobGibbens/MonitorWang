using System;
using System.Linq;
using MonitorWang.Core.Interfaces;
using Castle.Core;

namespace MonitorWang.Core.Loaders
{
    /// <summary>
    /// This will scan all assemblies & exes for matching types that
    /// implement the interface <typeparam name="TI"></typeparam>
    /// </summary>
    public class ScanningLoader<TI> : ILoader<TI>
    {
        /// <summary>
        /// Helper to directly use this components
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool Resolve(out TI[] components)
        {
            return new ScanningLoader<TI>().Load(out components);
        }

        /// <summary>
        /// Helper to directly use this components
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool Resolve(out TI[] components, Action<TI> action)
        {
            return new ScanningLoader<TI>().Load(out components, action);
        }

        public bool Load(out TI[] components)
        {
            components = new TI[0];
            Type[] matchingTypes;

            if (!TypeDiscovery.Discover<TI>(out matchingTypes))
                return false;

            var candidates = from type in matchingTypes
                             select (TI) Activator.CreateInstance(type);

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
    public class ScanningPluginLoader<TI> : ILoader<TI>
        where TI : IPlugin
    {
        /// <summary>
        /// Helper to directly use this components
        /// </summary>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool Resolve(out TI[] components)
        {
            return new ScanningPluginLoader<TI>().Load(out components);
        }

        /// <summary>
        /// Helper to directly use this components
        /// </summary>
        /// <param name="components"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool Resolve(out TI[] components, Action<TI> action)
        {
            return new ScanningPluginLoader<TI>().Load(out components, action);
        }

        public bool Load(out TI[] components)
        {
            components = new TI[0];
            Type[] matchingTypes;

            if (!TypeDiscovery.Discover<TI>(out matchingTypes))
                return false;

            var candidates = from type in matchingTypes
                             select (TI) Activator.CreateInstance(type)
                             into instance
                             select instance;

            var plugins = candidates.Cast<IPlugin>()
                .InitialisePlugins();

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