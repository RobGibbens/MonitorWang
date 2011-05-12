
namespace MonitorWang.Core.Interfaces.Entities
{
    /// <summary>
    /// This provides data to identify this agent
    /// </summary>
    public class AgentInfo
    {
        /// <summary>
        /// The installation this agent is collecting data for
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Identifies the agent, usually this would be the
        /// machine name
        /// </summary>
        public string AgentId { get; set; }
    }
}