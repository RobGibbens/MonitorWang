using System;
using System.Collections.Generic;
using NServiceBus;

namespace MonitorWang.Core.Containers
{
    public interface IContainer
    {
        IContainer RegisterAsTransient(Type implType);
        IContainer RegisterAsTransient<T>(Type implType);
        IContainer RegisterAsSingleton(Type implType);
        IContainer RegisterAsSingleton<T>(Type implType);
        IContainer RegisterInstance<T>(T instance);
        IContainer RegisterAll<T>();
        IContainer RegisterAllWithInterception<T, I>();
        object Resolve(string componentName);
        T Resolve<T>();        
        T[] ResolveAll<T>();
        T Find<T>(Func<IEnumerable<T>, T> filter);
        void ResolveAll<T>(Action<T> action);
        bool IsRegistered<T>();

        Configure Bus();
        Configure Bus(params string[] assemblyNames);
    }
}