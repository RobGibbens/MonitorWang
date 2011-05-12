using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Interfaces
{
    public interface IServicePlugin : IPlugin
    {
        /// <summary>
        /// Used to identify a plugin
        /// </summary>
        PluginDescriptor Identity { get; }

        void Start();
        void Stop();
        void Pause();
        void Continue();
    }
}