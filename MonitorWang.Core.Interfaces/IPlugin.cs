
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Interfaces
{
    public interface IPlugin
    {
        /// <summary>
        /// Describes the status of the plugin
        /// </summary>
        Status Status { get; set; }
        /// <summary>
        /// Called when a plugin is first loaded. Use this to do any one-time 
        /// setup/init check or operation prior to it starting its normal operation
        /// </summary>
        void Initialise();
    }
}