using System;

namespace MonitorWang.Agent.Profiles
{
    /// <summary>
    /// 
    /// </summary>
    public class FastStartAgentProfile : DefaultAgentProfile
    {
        public override string Name
        {
            get { return "FastStartAgent"; }
        }

        public override Type DefineRole()
        {
            return typeof (Roles.FastStartAgent);
        }
    }
}