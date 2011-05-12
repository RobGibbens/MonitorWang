using System;
using System.Collections.Generic;
using System.Linq;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;
using Castle.Core;

namespace MonitorWang.Core.Loaders
{
    /// <summary>
    /// This will load all binding configurations from IoC and convert them into 
    /// Bindings. It will also scan the application binaries to load all IBinding
    /// instances
    /// </summary>
    public class HealthCheckBindingLoader : ILoader<Binding>
    {
        public bool Load(out Binding[] components)
        {
            // 1. Load all BindingConfiguration components from IoC
            // 2. ForEach BindingConfiguration build an IBinding instance
            //    HealthCheck ============================================
            //      o Load named check config component from IoC
            //      o Infer type associated with it
            //      o Create instance of check type (check config ctor)
            //    Scheduler ==============================================
            //      o Load named scheduler config component from IoC
            //      o Infer scheduler type associated with it
            //      o Create instance of scheduler type (scheduler config, check)
            // 3. Scan and CreateInstance of all <IBinding> implementations

            components = new Binding[0];
            BindingConfiguration[] bindingConfigs;

            if (!ContainerLoader<BindingConfiguration>.Resolve(out bindingConfigs))
                return false;

            var bindings = new List<Binding>();

            bindingConfigs.ToList().ForEach(bc =>
                                                {
                                                    var checkConfig = Container.Resolve(bc.HealthCheckConfigurationName);

                                                    var configSupportsEnabling = checkConfig as ICanBeSwitchedOff;
                                                    if ((configSupportsEnabling != null) && !configSupportsEnabling.Enabled)
                                                        // skip this plugin, not enabled
                                                        return;

                                                    // Load healthcheck.............
                                                    var inferredCheckTypeName = checkConfig.GetType().Name.TrimEnd("config");
                                                                                                            
                                                    Type checkType;
                                                    if (!GetType<IHealthCheckPlugin>(inferredCheckTypeName, out checkType))
                                                        throw new InvalidOperationException(
                                                            string.Format(
                                                                "Searching for type name '{0}'; found no matches. Check the HealthCheckConfigurationName property of your BindingConfigurations are valid",
                                                                inferredCheckTypeName));

                                                    var check =
                                                        Activator.CreateInstance(checkType, checkConfig) as
                                                        IHealthCheckPlugin;

                                                    // Load scheduler.............
                                                    var schedulerConfig = Container.Resolve(bc.ScheduleConfigurationName);

                                                    var inferredSchedulerTypeName = schedulerConfig.GetType().Name.Replace("Config", string.Empty);

                                                    Type schedulerType;
                                                    if (!GetType<IHealthCheckSchedulerPlugin>(inferredSchedulerTypeName, out schedulerType))
                                                        throw new InvalidOperationException(
                                                            string.Format(
                                                                "Searching for type name '{0}'; found no matches. Check the ScheduleConfigurationName property of your BindingConfigurations are valid",
                                                                inferredSchedulerTypeName));

                                                    var scheduler =
                                                        Activator.CreateInstance(schedulerType, check, schedulerConfig) as
                                                        IHealthCheckSchedulerPlugin;

                                                    bindings.Add(new Binding
                                                                     {
                                                                         Check = check,
                                                                         Scheduler = scheduler
                                                                     });
                                                });

            // Finally load any programmatic bindings.............
            Binding[] codeBindings;
            if (ScanningLoader<Binding>.Resolve(out codeBindings))
                bindings.Concat(codeBindings);

            components = bindings.ToArray();
            return (components.Count() > 0);
        }

        public bool Load(out Binding[] components, Action<Binding> action)
        {
            var result = Load(out components);
            components.ForEach(action);
            return result;
        }

        private static bool GetType<T>(string targetTypeName, out Type targetType)
        {
            targetType = null;
            Type[] matchingTypes;

            if (!TypeDiscovery.Discover<T>(out matchingTypes,
                t => string.Compare(t.Name, targetTypeName,
                    StringComparison.InvariantCultureIgnoreCase) == 0))
                return false;

            if (matchingTypes.Count() != 1)
                throw new InvalidOperationException(string.Format("Searching for type '{0}' named '{1}'; found {2} matches, expected only 1",
                    typeof(T).Name,
                    targetTypeName, 
                    matchingTypes.Count()));

            targetType = matchingTypes.First();
            return true;
        }
    }
}