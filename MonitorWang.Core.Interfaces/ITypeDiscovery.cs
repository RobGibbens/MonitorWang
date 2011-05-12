using System;

namespace MonitorWang.Core.Interfaces
{
    public interface ITypeDiscovery
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceTypes"></param>
        /// <param name="matchingTypes"></param>
        /// <returns></returns>
        bool Locate(string[] interfaceTypes, out Type[] matchingTypes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceTypes"></param>
        /// <param name="matchingTypes"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        bool Locate(string[] interfaceTypes, out Type[] matchingTypes, Predicate<Type> filter);
    }
}