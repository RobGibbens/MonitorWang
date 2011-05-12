using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Configuration.Interpreters;
using NServiceBus;

namespace MonitorWang.Core.Containers
{
    public class WindsorContainer : IContainer
    {        
        protected static Castle.Windsor.WindsorContainer myInstance =
            new Castle.Windsor.WindsorContainer(new ZeroAppConfigXmlInterpreter());

        public IContainer RegisterAsTransient(Type implType)
        {
            myInstance.Register(Component.For(implType)
                .ImplementedBy(implType)
                .LifeStyle.Transient);
            return this;
        }

        public IContainer RegisterAsTransient<T>(Type implType)
        {
            myInstance.Register(Component.For<T>()
                .ImplementedBy(implType)
                .LifeStyle.Transient);
            return this;
        }

        public IContainer RegisterAsSingleton(Type implType)
        {
            myInstance.Register(Component.For(implType)
                .ImplementedBy(implType)
                .LifeStyle.Singleton);
            return this;
        }

        public IContainer RegisterAsSingleton<T>(Type implType)
        {
            myInstance.Register(Component.For<T>()
                .ImplementedBy(implType)
                .LifeStyle.Singleton);
            return this;
        }

        public IContainer RegisterInstance<T>(T instance)
        {
            myInstance.Register(Component.For<T>().Instance(instance));
            return this;
        }

        public IContainer RegisterAll<T>()
        {
            Type[] components;

            if (TypeDiscovery.Discover<T>(out components))
                components.ForEach(c => RegisterAsTransient<T>(c));            
            return this;
        }

        public IContainer RegisterAllWithInterception<T, I>()
        {
            Type[] components;

            if (!TypeDiscovery.Discover<T>(out components))
                return this;

            var interceptorTypes = (from iType in ResolveAll<I>()
                                    select iType.GetType()).ToArray();

            components.ForEach(c =>
                               myInstance.Register(Component.For(typeof (T))
                                                       .LifeStyle.Transient
                                                       .ImplementedBy(c)
                                                       .Interceptors(interceptorTypes)));
            return this;
        }

        public object Resolve(string componentName)
        {
            return myInstance.Resolve(componentName, new Dictionary<string, string>());
        }

        public T Resolve<T>()
        {
            return myInstance.Resolve<T>();
        }

        public T[] ResolveAll<T>()
        {
            return myInstance.ResolveAll<T>();
        }

        public T Find<T>(Func<IEnumerable<T>, T> filter)
        {
            var components = ResolveAll<T>();
            return filter(components);
        }

        public void ResolveAll<T>(Action<T> action)
        {
            var components = ResolveAll<T>();
            components.ForEach(action);
        }

        public bool IsRegistered<T>()
        {
            return myInstance.Kernel.HasComponent(typeof (T));
        }

        public Configure Bus()
        {
            return Configure.With().CastleWindsorBuilder(myInstance);
        }

        public Configure Bus(params string[] assemblyNames)
        {
            var assemblies = from assemblyName in assemblyNames
                             select Assembly.Load(assemblyName);
            
            var listOfAssemblies = assemblies.ToList();

            // these are pre-requesites to allow NSB infrastructure
            // messaging (subscription etc) to operate
            listOfAssemblies.Add(Assembly.Load("NServiceBus"));
            listOfAssemblies.Add(Assembly.Load("NServiceBus.Core"));
            // finally register the assemblies with NSB and
            // register NSB with our Windsor container so that
            // any IBus dependencies are automatically resolved
            return Configure.With(listOfAssemblies).CastleWindsorBuilder(myInstance);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>http://stackoverflow.com/questions/317981/can-castle-windsor-locate-files-in-a-subdirectory</remarks>
    public class ZeroAppConfigXmlInterpreter : XmlInterpreter
    {
        public override void ProcessResource(IResource source, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
        {
            // default stuff...
            base.ProcessResource(source, store);

            // custom stuff..auto register all config\*.castle.config files
            var configFilesLocation = SmartLocation.GetLocation("config");

            foreach (var extraConfig in Directory.GetFiles(configFilesLocation, "*.castle.config"))
            {
                try
                {
                    var interpreter = new XmlInterpreter(extraConfig) { Kernel = Kernel };
                    interpreter.ProcessResource(interpreter.Source, store);
                }
                catch (ConfigurationErrorsException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to load configuration: " + extraConfig, ex);
                }
            }
        }
    }
}