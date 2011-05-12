using System;
using MonitorWang.Core.Geckoboard.DataProvider;
using MonitorWang.Core.Geckoboard.Entities;

namespace MonitorWang.Core.Geckoboard
{
    public partial class GeckoboardDataServiceImpl : IGeckoboardDataServiceImpl
    {
        protected IGeckoboardDataProvider myDataProvider;
        protected IColourPicker myColourPicker;

        public GeckoboardDataServiceImpl(IGeckoboardDataProvider dataProvider,
            IColourPicker colourPicker)
        {
            myDataProvider = dataProvider;
            myColourPicker = colourPicker;
        }

        /// <summary>
        /// This will get the min, max and average resultcount for a specific site and check
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual GeckoMeter GetGeckoboardGeckoMeterForSiteCheck(GeckometerArgs args)
        {
            var data = new GeckoMeter
            {
                DecimalPlaces = args.DecimalPlaces
            };

            try
            {
                var rawData = myDataProvider.GetGeckoMeterDataForSiteCheck(args);

                data.Item = rawData.Avg;
                data.Min.Text = "Min";
                data.Min.Value = rawData.Min;
                data.Max.Text = "Max";
                data.Max.Value = rawData.Max;
            }
            catch (Exception ex)
            {
                Logger.Error(Logger.Event.During("GetGeckoboardGeckoMeterForSiteCheck")
                    .Encountered(ex));
                throw;
            }

            return data;
        }               
        
        /// <summary>
        /// This will get the last and previous to last resultcount for a specific site and check
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual GeckoComparison GetGeckoboardComparisonForSiteCheck(ComparisonArgs args)
        {
            var data = new GeckoComparison(args.DecimalPlaces);

            try
            {
                var rawData = myDataProvider.GetComparisonDataForSiteCheck(args);
                data.Number.Value = rawData.Number;
                data.Comparison.Value = rawData.Comparison;
            }
            catch (Exception ex)
            {
                Logger.Error(Logger.Event.During("GetGeckoboardComparisonForSiteCheck")
                    .Encountered(ex));
                throw;
            }

            return data;
        }

        /// <summary>
        /// This will get the number of days interval from or to a date supplied
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public virtual GeckoComparison GetGeckoboardDayInterval(string date)
        {
            var data = new GeckoComparison();

            try
            {
                DateTime targetDate;

                if (!DateTime.TryParse(date, out targetDate))
                {
                    var res = new GeckoComparison
                                  {
                                      Number = {Value = -1}
                                  };
                    return res;
                }

                // remove the time portion
                var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                data.Number.Value = (long) targetDate.Subtract(now).TotalDays;
                data.Number.Text = "Days";
            }
            catch (Exception ex)
            {
                Logger.Error(Logger.Event.During("GetGeckoboardComparisonForSiteCheck")
                    .Encountered(ex));
                throw;
            }

            return data;
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