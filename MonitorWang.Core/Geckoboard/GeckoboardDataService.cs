using System;
using System.Collections.Generic;
using System.ServiceModel.Web;
using Microsoft.ServiceModel.Web;
using MonitorWang.Core.Geckoboard.DataProvider;
using MonitorWang.Core.Geckoboard.Entities;
using System.Linq;

namespace MonitorWang.Core.Geckoboard
{
    /// <summary>
    /// Provides the configuration for the Geckoboard Data Service. By 
    /// default the Data Provider is SQLite.
    /// </summary>
    public class GeckoboardDataServiceConfig
    {
        public string DataProvider { get; set; }
    }

    public class GeckoboardDataService : IGeckoboardDataService
    {
        public const int MAX_LINECHART_ITEMS = 300;
        public const string GeckoboardDataProviderPrefix = "GeckoboardDataProvider-";

        protected static GeckoboardDataServiceConfig myConfig;        
        protected static IGeckoboardDataServiceImpl myImplementation;

        static GeckoboardDataService()
        {
            // we can create a custom WCF servicehost to hook into the IoC container
            // to get an instance of this class - this would mean auto wiring of dependencies
            // however we are using the REST starter kit servicehost so we would need to be able
            // to subclass this - more investigation required....in the meantime we will explicitly
            // call the container to get our dependent components
            myConfig = Container.Resolve<GeckoboardDataServiceConfig>();
            // this will find the IDataProvider that has a matching name 
            // as specified in the configuration
            var dataProvider = Container.Find<IGeckoboardDataProvider>(FindMyProvider);
            // this will boot up our colour picker component
            var colourPicker = Container.Resolve<IColourPicker>();
            // finally lets create our actual component that will do the real work
            // The implementation component is split from this class to help improve
            // testability - this class merely provides the WCF glue            
            myImplementation = new GeckoboardDataServiceImpl(dataProvider, colourPicker);
        }

        private static IGeckoboardDataProvider FindMyProvider(IEnumerable<IGeckoboardDataProvider> providers)
        {
            var targetId = string.Format("{0}{1}", GeckoboardDataProviderPrefix, myConfig.DataProvider);
            return providers.First(dp => string.Compare(dp.FriendlyId, targetId, true) == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebHelp(Comment = "This will return geckoboard piechart data for...")]
        [WebInvoke(UriTemplate = "piechart/sites", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public virtual GeckoPieChart GetGeckoboardPieChartForAllSites()
        {
            Caching.NoCache();            
            return myImplementation.GetGeckoboardPieChartForAllSites();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebHelp(Comment = "This will return geckoboard piechart data for...")]
        [WebInvoke(UriTemplate = "piechart/sites/{site}", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public virtual GeckoPieChart GetGeckoboardPieChartForSite(string site)
        {
            Caching.NoCache();
            return myImplementation.GetGeckoboardPieChartForSite(site);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [WebHelp(Comment = "This will return geckoboard piechart data for...")]
        [WebInvoke(UriTemplate = "piechart/{check}/{outcome}/{operation}", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public GeckoPieChart GetGeckoboardPieChartForCheck(string check, string outcome, string operation)
        {
            var operationType = (DataOperationType) Enum.Parse(typeof (DataOperationType), operation, true);
            var outcomeType = (OutcomeType)Enum.Parse(typeof(OutcomeType), outcome, true);

            Caching.NoCache();
            return myImplementation.GetGeckoboardPieChartForCheck(new PieChartArgs
                                                                          {
                                                                              Site = QueryString.AsString("site"),
                                                                              Agent = QueryString.AsString("agent"),
                                                                              Check = check,
                                                                              Outcome = outcomeType,
                                                                              DataOperation = operationType
                                                                          });
        }

        [WebHelp(Comment = "This will geckoboard linechart data for...")]
        [WebInvoke(UriTemplate = "linechart/{check}/{outcome}/{operation}/per/{rate}", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public GeckoLineChart GetGeckoboardLineChartForCheckRate(string check, string outcome, string operation, string rate)
        {
            var operationType = (DataOperationType)Enum.Parse(typeof (DataOperationType), operation, true);
            var outcomeType = (OutcomeType)Enum.Parse(typeof(OutcomeType), outcome, true);

            Caching.NoCache();
            var data = myImplementation.GetGeckoboardLineChartForCheckRate(new LineChartArgs
            {
                Site = QueryString.AsString("site"),
                Agent = QueryString.AsString("agent"),
                Tag = QueryString.AsString("tag"),
                Check = check,
                Unit = rate,
                Limit = QueryString.AsInt("limit", MAX_LINECHART_ITEMS),
                MaxItems = MAX_LINECHART_ITEMS,
                Sample = QueryString.AsInt("sample", 1),
                ScaleYDecimalPlaces = GetDecimalPlacesFromRequest(),
                DataOperation = operationType,
                DecimalPlaces = GetDecimalPlacesFromRequest(),
                Outcome = outcomeType
            });

            return data;
        }

        /// <summary>
        /// This will get the min, max and average resultcount for a specific site and check
        /// </summary>
        /// <param name="site"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        [WebHelp(Comment = "This will geckoboard geck-o-meter data for...")]
        [WebInvoke(UriTemplate = "geckometer/sites/{site}/{check}", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public virtual GeckoMeter GetGeckoboardGeckoMeterForSiteCheck(string site, string check)
        {
            Caching.NoCache();
            return myImplementation.GetGeckoboardGeckoMeterForSiteCheck(new GeckometerArgs
            {
                Check = check,
                Site = site,
                Tag = QueryString.AsString("tag"),
                DecimalPlaces = GetDecimalPlacesFromRequest()
            });
        }               
        
        /// <summary>
        /// This will get the last and previous to last resultcount for a specific site and check
        /// </summary>
        /// <param name="site"></param>
        /// <param name="check"></param>
        /// <returns></returns>
        [WebHelp(Comment = "This will geckoboard linechart data for...")]
        [WebInvoke(UriTemplate = "comparison/sites/{site}/{check}", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public virtual GeckoComparison GetGeckoboardComparisonForSiteCheck(string site, string check)
        {
            Caching.NoCache();
            return myImplementation.GetGeckoboardComparisonForSiteCheck(new ComparisonArgs
            {
                Check = check,
                Site = site,
                Tag = QueryString.AsString("tag"),
                DecimalPlaces = GetDecimalPlacesFromRequest()
            });
        }

        /// <summary>
        /// This will get the number of days interval from or to a date supplied
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [WebHelp(Comment = "This will the number of days from today and the date specified...")]
        [WebInvoke(UriTemplate = "days/{date}", ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        public virtual GeckoComparison GetGeckoboardDayInterval(string date)
        {
            Caching.NoCache();
            return myImplementation.GetGeckoboardDayInterval(date);
        }

        protected virtual int GetDecimalPlacesFromRequest()
        {
            return QueryString.AsInt("dp", 0);
        }

        protected double Scale(double value, int scaleup, int scaledown, int dp)
        {
            if (scaleup != 1)
                return Math.Round(value * scaleup, dp);
            if (scaledown != 1)
                return Math.Round(value * scaledown, dp);

            return Math.Round(value, dp);
        }
    }
}