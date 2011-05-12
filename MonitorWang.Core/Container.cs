using System;
using System.Collections.Generic;
using MonitorWang.Core.Containers;
using NServiceBus;

namespace MonitorWang.Core
{
    public class Container
    {
        protected static IContainer myContainer = new WindsorContainer();

        public static IContainer RegisterAsTransient(Type implType)
        {
            return myContainer.RegisterAsTransient(implType);
        }

        public static IContainer RegisterAsTransient<T>(Type implType)
        {
            return myContainer.RegisterAsTransient<T>(implType);
        }

        public static IContainer RegisterAsSingleton(Type implType)
        {
            return myContainer.RegisterAsSingleton(implType);
        }

        public static IContainer RegisterAsSingleton<T>(Type implType)
        {
            return myContainer.RegisterAsSingleton<T>(implType);
        }

        public static IContainer RegisterInstance<T>(T instance)
        {
            return myContainer.RegisterInstance(instance);
        }

        public static IContainer RegisterAll<T>()
        {
            return myContainer.RegisterAll<T>();
        }

        public static IContainer RegisterAllWithInterception<T, I>()
        {
            return myContainer.RegisterAllWithInterception<T, I>();
        }

        public static object Resolve(string componentName)
        {
            return myContainer.Resolve(componentName);
        }

        public static T Resolve<T>()
        {
            return myContainer.Resolve<T>();
        }

        public static T[] ResolveAll<T>()
        {
            return myContainer.ResolveAll<T>();
        }

        /// <summary>
        /// This will resolve all implementations of the interface
        /// specified but then filters these through the delegate
        /// supplied to pick a specific implementation to use
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static T Find<T>(Func<IEnumerable<T>, T> filter)
        {
            return myContainer.Find(filter);
        }

        public static void ResolveAll<T>(Action<T> action)
        {
            myContainer.ResolveAll(action);
        }

        public static bool IsRegistered<T>()
        {
            return myContainer.IsRegistered<T>();
        }

        public static Configure Bus()
        {
            return myContainer.Bus();
        }

        public static Configure Bus(params string[] assemblyNames)
        {
            return myContainer.Bus(assemblyNames);
        }
    }
}