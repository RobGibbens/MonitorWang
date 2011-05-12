using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core
{
    /// <summary>
    /// This will scan all the assemblies (*.dll|exe) in a given folder for a given set 
    /// of types and will return all the Type(s) that implement the interfaces specified
    /// </summary>
    public class TypeDiscovery : ITypeDiscovery
    {
        protected static readonly string myBinFolder;
        protected static readonly TypeDiscoveryConfig myConfig;
        protected static List<string> myFilesToExclude;

        static TypeDiscovery()
        {
            myBinFolder = SmartLocation.GetBinFolder();
            myConfig = Container.Resolve<TypeDiscoveryConfig>();

            myFilesToExclude = new List<string>();
            var exclude = new List<string>();

            if (myConfig.Exclude == null) 
                return;

            myConfig.Exclude.ForEach(e => exclude.AddRange(Directory.GetFiles(myBinFolder, e)));
            myFilesToExclude = exclude.Distinct().ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TypeDiscovery()
        {
        }

        /// <summary>
        /// Simple helper to locate a single type in *.dll|exe
        /// </summary>
        /// <param name="matchingTypes"></param>
        /// <returns></returns>
        public static bool Discover<T>(out Type[] matchingTypes)
        {
            return Discover<T>(out matchingTypes, t => true);
        }

        /// <summary>
        /// Simple helper to locate a single type in *.dll|exe
        /// </summary>
        /// <param name="matchingTypes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool Discover<T>(out Type[] matchingTypes, Predicate<Type> filter)
        {
            return Discover(out matchingTypes, filter, typeof(T).Name);
        }

        /// <summary>
        /// Simple helper to locate matching types in *.dll|exe
        /// </summary>
        /// <param name="interfaceTypes"></param>
        /// <param name="matchingTypes"></param>
        /// <returns></returns>
        public static bool Discover(out Type[] matchingTypes, params string[] interfaceTypes)
        {
            return new TypeDiscovery().Locate(interfaceTypes, out matchingTypes);
        }

        /// <summary>
        /// Simple helper to locate matching types in *.dll|exe
        /// </summary>
        /// <param name="interfaceTypes"></param>
        /// <param name="matchingTypes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static bool Discover(out Type[] matchingTypes, Predicate<Type> filter, params string[] interfaceTypes)
        {
            return new TypeDiscovery().Locate(interfaceTypes, out matchingTypes, filter);
        }

        /// <summary>
        /// Simple helper to locate matching types in *.dll|exe
        /// </summary>
        /// <param name="interfaceTypes"></param>
        /// <param name="matchingTypes"></param>
        /// <returns></returns>
        public static bool Discover(out Type[] matchingTypes, params Type[] interfaceTypes)
        {
            return Discover(out matchingTypes, x => true, interfaceTypes);
        }

        public static bool Discover(out Type[] matchingTypes, Predicate<Type> filter, params Type[] interfaceTypes)
        {
            var typeNames = from interfaceType in interfaceTypes
                            select interfaceType.Name;

            return new TypeDiscovery().Locate(typeNames.ToArray(), out matchingTypes, filter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceTypes"></param>
        /// <param name="matchingTypes"></param>
        /// <returns></returns>
        public bool Locate(string[] interfaceTypes, out Type[] matchingTypes)
        {
            return Locate(interfaceTypes, out matchingTypes, x => true);
        }

        public bool Locate(string[] interfaceTypes, out Type[] matchingTypes, Predicate<Type> filter)
        {
            Logger.Debug("Scanning for Interface Types: {0}", string.Join(",", interfaceTypes));

            var candidates = Directory.GetFiles(myBinFolder, "*.dll", SearchOption.TopDirectoryOnly).Concat(
                Directory.GetFiles(myBinFolder, "*.exe", SearchOption.TopDirectoryOnly));
            var assembliesToScan = candidates.Where(
                filename => !myFilesToExclude.Contains(filename));

            var implementingTypes = from assemblyName in assembliesToScan
                                    select Assembly.LoadFile(assemblyName)
                                        into assembly
                                        from type in assembly.GetExportedTypes()
                                        where filter(type) &&
                                        !type.IsAbstract &&
                                        (Array.Find(interfaceTypes, interfaceToFind =>                                            
                                            (type.GetInterface(interfaceToFind, true) != null)) != null)
                                        select type;

            matchingTypes = implementingTypes.ToArray();
            return (matchingTypes.Length > 0);
        }
    }
}