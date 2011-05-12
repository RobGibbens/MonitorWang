namespace MonitorWang.Core.Interfaces
{
    /// <summary>
    /// This is a marker interface - its allows us to differentiate
    /// a "startup" plugin from other types
    /// </summary>
    public interface IStartupPlugin : IPlugin
    {
        
    }
}