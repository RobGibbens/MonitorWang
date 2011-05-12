namespace MonitorWang.Core.Interfaces
{
    /// <summary>
    /// A profile enables rapid configuration of an agent. This involves
    /// loading the <see cref="Container"/> with the correct components
    /// </summary>
    public interface IRoleProfile
    {
        string Name { get; }
        IRolePlugin Role { get; }
    }
}