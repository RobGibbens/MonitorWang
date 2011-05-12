using MonitorWang.Core;
using MonitorWang.Core.Interfaces;

namespace MonitorWang.Agent.Profiles
{
    /// <summary>
    /// Provides a common profile base functionality - this primarily
    /// consists of help constructing and registering publishers into
    /// the IoC container
    /// </summary>
    public abstract class ProfileBase : IRoleProfile
    {
        public abstract string Name { get; }
        public abstract void CreateRole();

        public IRolePlugin Role
        {
            get
            {
                // load and execute all startup plugins
                // ...not using .SafeInitialise() method as
                // we want this to blow up loading the agent
                Container.RegisterAll<IStartupPlugin>()
                    .ResolveAll<IStartupPlugin>(c => c.InitialiseIfEnabled());

                // hook to create custom role components
                CreateRole();

                // finally resolve the role component
                return Container.Resolve<IRolePlugin>();
            }
        }
    }
}