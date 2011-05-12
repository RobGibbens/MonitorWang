using System;
using System.Collections.Generic;

namespace MonitorWang.Core.Interfaces.Entities
{
    [Serializable]
    public class HealthCheckAgentStart
    {
        /// <summary>
        /// A unique identifier for this message
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Allows us to identify the agent that is starting up
        /// </summary>
        public AgentInfo Agent { get; set; }
        /// <summary>
        /// The timestamp when we started the discovery of checks to execute
        /// </summary>
        public DateTime DiscoveryStarted { get; set; }
        /// <summary>
        /// The timestamp when we completed the discovery of checks to execute
        /// </summary>
        public DateTime DiscoveryCompleted { get; set; }
        /// <summary>
        /// The list of checks we will be executing this session
        /// </summary>
        public List<PluginDescriptor> Checks { get; set; }
        /// <summary>
        /// The list of unhealthy checks - these failed to initialise
        /// </summary>
        public List<PluginDescriptor> UnhealthyChecks { get; set; }
        /// <summary>
        /// The list of activities we will be executing this session
        /// </summary>
        public List<PluginDescriptor> Activities { get; set; }
        /// <summary>
        /// The list of unhealthy activities - these failed to initialise
        /// </summary>
        public List<PluginDescriptor> UnhealthyActivities { get; set; }

        /// <summary>
        /// default ctor
        /// </summary>
        public HealthCheckAgentStart()
        {
            Id = Guid.NewGuid();
        }
    }
}