namespace MonitorWang.Core.Interfaces
{
    public interface IProfileLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        bool Load(string name, out IRoleProfile profile);
    }
}