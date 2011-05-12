using System;
using System.Linq;
using MonitorWang.Core.Interfaces;

namespace MonitorWang.Core.Loaders
{
    /// <summary>
    /// This will attempt to return an instance of the <see cref="IRoleProfile"/>
    /// implementation that matches the name specified
    /// </summary>
    public class ProfileLoader : IProfileLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static bool Find(string name, out IRoleProfile profile)
        {
            return new ProfileLoader().Load(name, out profile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        public bool Load(string name, out IRoleProfile profile)
        {
            profile = null;
            Type[] matchingTypes;

            if (!TypeDiscovery.Discover<IRoleProfile>(out matchingTypes))
                return false;

            var profileTypeName = (name.EndsWith("Profile")) ? name : name + "Profile";
            var profileType = matchingTypes.First(candidateType => string.Compare(candidateType.Name, profileTypeName) == 0);
            profile = (IRoleProfile) Activator.CreateInstance(profileType);
            return (profile != null);
        }
    }
}