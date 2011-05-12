using System.ServiceModel;
using MonitorWang.Core.Geckoboard.Entities;

namespace MonitorWang.Core.Geckoboard
{
    [ServiceContract(Namespace = "http://monitorwang.iagileservices.com/dataservices/geckoboard")]
    public interface IGeckoboardDataService
    {
        /// <summary>
        /// piechart/sites
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GeckoPieChart GetGeckoboardPieChartForAllSites();
        /// <summary>
        /// piechart/sites/{site}
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        [OperationContract]
        GeckoPieChart GetGeckoboardPieChartForSite(string site);
        /// <summary>
        /// piechart/{check}/{outcome}/{operation}
        /// where 
        /// outcome = all|failure|success
        /// operation = average|sum|count
        /// </summary>
        /// <param name="check"></param>
        /// <param name="outcome"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        [OperationContract]
        GeckoPieChart GetGeckoboardPieChartForCheck(string check, string outcome, string operation);

        /// <summary>
        /// linechart/{check}/{outcome}/{operation}/per/{rate}
        /// where 
        /// outcome = all|failure|success
        /// operation = average|sum|count
        /// </summary>
        /// <param name="check"></param>
        /// <param name="outcome"></param>
        /// <param name="operation"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        [OperationContract]
        GeckoLineChart GetGeckoboardLineChartForCheckRate(string check, string outcome, string operation, string rate);

        /// <summary>
        /// geckometer/sites/{site}/{check}
        /// </summary>
        /// <param name="site"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        [OperationContract]
        GeckoMeter GetGeckoboardGeckoMeterForSiteCheck(string site, string check);
        /// <summary>
        /// comparison/sites/{site}/{check}
        /// </summary>
        /// <param name="site"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        [OperationContract]
        GeckoComparison GetGeckoboardComparisonForSiteCheck(string site, string check);

        [OperationContract]
        GeckoComparison GetGeckoboardDayInterval(string date);
    }
}