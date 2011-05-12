namespace MonitorWang.Core.AppStats
{
    /// <summary>
    /// This demonstrates how you can add/extend the fluent helper methods of the 
    /// AppStatsEvent* classes. To provide your own extension methods create a new
    /// ext. method and define your own continuation interface(s) which should eventually
    /// return the original AppStatsEvent object back. Create a new class to take as a ctor 
    /// param the AppStatsEvent object passed to the ext. method - this should also explicitly
    /// implement your continuation interface(s)
    /// </summary>
    public static class AppStatsEventExtensions
    {            
        public static IAppStatsPieChartContinuation<AppStatsEvent> PieChart(this AppStatsEvent stat, string id)
        {
            stat.CheckId = id;
            return stat;
        }

        public static IAppStatsPieChartContinuation<AppStatsEngine.AppStatsEventTimer> PieChart(this AppStatsEngine.AppStatsEventTimer stat, string id)
        {
            stat.CheckId = id;
            return stat;
        }

        public interface IAppStatsPieChartContinuation<T>
        {
            T Segment(string id);
        }
    }
}