using MonitorWang.Core.Geckoboard.DataProvider;
using MonitorWang.Core.Geckoboard.Entities;

namespace MonitorWang.Core.Geckoboard
{
    public interface IGeckoboardDataServiceImpl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        GeckoPieChart GetGeckoboardPieChartForAllSites();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        GeckoPieChart GetGeckoboardPieChartForSite(string site);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        GeckoPieChart GetGeckoboardPieChartForCheck(PieChartArgs args);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        GeckoLineChart GetGeckoboardLineChartForCheckRate(LineChartArgs args);

        /// <summary>
        /// This will get the min, max and average resultcount for a specific site and check
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        GeckoMeter GetGeckoboardGeckoMeterForSiteCheck(GeckometerArgs args);

        /// <summary>
        /// This will get the last and previous to last resultcount for a specific site and check
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        GeckoComparison GetGeckoboardComparisonForSiteCheck(ComparisonArgs args);

        /// <summary>
        /// This will get the number of days interval from or to a date supplied
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        GeckoComparison GetGeckoboardDayInterval(string date);
    }
}