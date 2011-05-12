using System;
using System.Collections.Generic;
using MonitorWang.Core.Geckoboard.DataProvider;
using MonitorWang.Core.Geckoboard.Entities;
using System.Linq;

namespace MonitorWang.Core.Geckoboard
{
    public partial class GeckoboardDataServiceImpl
    {
        public class Linechart
        {
            public const string X_Axis_DateFormat = "d-MMM/HH.mm";    
        }

        public GeckoLineChart GetGeckoboardLineChartForCheckRate(LineChartArgs args)
        {
            GeckoLineChart data = null;

            try
            {
                var yAxis = new GeckoValues();
                var xAxis = new List<DateTime>();

                CalculateLinechartSample(args);

                DateTime oldestDate;
                DateTime newestDate;
                CalculateLinechartDateRange(args, out oldestDate, out newestDate);

                // get the actual data from the provider and a "zero set" - a set
                // of zero values for the entire date range
                var dbData = myDataProvider.GetLineChartDataForCheckRate(args);
                var zeroData = GenerateLinechartZeroSet(args, oldestDate, newestDate);
                // merge the two data sets, using the actual data value where
                // available, otherwise use a zero value
                var mergedData = MergeLinechartData(dbData, zeroData);
                data = BuildLinechartData(args, mergedData, xAxis, yAxis);
                CalculateLinechartSettings(args, data, yAxis, xAxis);
            }
            catch (Exception ex)
            {
                Logger.Error(Logger.Event.During("GetGeckoboardLineChartForCheckRate")
                    .Encountered(ex));
                throw;
            }
            
            return data;
        }

        protected virtual GeckoLineChart BuildLinechartData(LineChartArgs args,
            IEnumerable<LineChartData> sourceData,  
            List<DateTime> xAxis, 
            GeckoValues yAxis)
        {
            var data = new GeckoLineChart();

            for (var i = 0; i < sourceData.Count(); i++)
            {
                if (i != 0 && (i % args.Sample != 0))
                    continue;

                // start at the newest and work backwards
                var j = sourceData.Count() - i - 1;
                var lci = sourceData.ElementAt(j);

                xAxis.Add(lci.When);
                yAxis.Add(lci.Value);
                data.Item.Add(Scale(lci.Value,
                                    args.ScaleUp,
                                    args.ScaleDown,
                                    args.DecimalPlaces).ToString());
            }

            // finally reverse the data order oldest->newest
            data.Item.Reverse();
            return data;
        }

        protected virtual IEnumerable<LineChartData> MergeLinechartData(IEnumerable<LineChartData> dbData, IEnumerable<LineChartData> zeroData)
        {
            // merge the two data sets, using the actual data value where
            // available, otherwise use a zero value
            return (from zeroItem in zeroData
                    select new LineChartData
                               {
                                   Value = (from dbItem in dbData
                                            where (zeroItem.When.CompareTo(dbItem.When) == 0)
                                            select dbItem.Value).DefaultIfEmpty(zeroItem.Value).ElementAtOrDefault(0),
                                   When = zeroItem.When
                               });
        }

        protected virtual IEnumerable<LineChartData> GenerateLinechartZeroSet(LineChartArgs args, DateTime oldestDate, DateTime newestDate)
        {
            const double value = 0;
            var data = new List<LineChartData>();

            switch (args.Unit.ToLowerInvariant())
            {
                case "day":
                    for (var i = args.Limit - 1; i >= 0; i--)
                    {
                        var baseDate = newestDate.Subtract(TimeSpan.FromDays(i));
                        data.Add(new LineChartData
                                     {
                                         Value = value,
                                         When = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day)
                                     });
                    }
                    break;
                case "hour":
                    for (var i = args.Limit - 1; i >= 0; i--)
                    {
                        var baseDate = newestDate.Subtract(TimeSpan.FromHours(i));
                        data.Add(new LineChartData
                                     {
                                         Value = value,
                                         When = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 
                                             baseDate.Hour, 0, 0)
                                     });
                    }
                    break;
                default:
                    for (var i = args.Limit - 1; i >= 0; i--)
                    {
                        var baseDate = newestDate.Subtract(TimeSpan.FromMinutes(i));
                        data.Add(new LineChartData
                                     {
                                         Value = value,
                                         When = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day,
                                             baseDate.Hour, baseDate.Minute, 0)
                                     });
                    }
                    break;
            }

            return data;
        }

        protected void CalculateLinechartSample(LineChartArgs args)
        {
            if ((args.Limit/args.Sample) <= args.MaxItems) 
                return;

            var s = (Convert.ToDouble(args.Limit) / Convert.ToDouble(args.MaxItems));
            var newSample = Math.Ceiling(s);
            args.Sample = Convert.ToInt32(newSample);

            Logger.Debug("Calculated sample freq. based on {0} limit: sample every {1} items",
                         args.Limit,
                         args.Sample);
        }
        /// <summary>
        /// This will calculate the date range we are generating the linechart for
        /// </summary>
        /// <param name="args"></param>
        /// <param name="oldestDate"></param>
        /// <param name="newestDate"></param>
        /// <returns></returns>
        protected void CalculateLinechartDateRange(LineChartArgs args, out DateTime oldestDate, out DateTime newestDate)
        {
            newestDate = args.EndDate;

            // work out the range
            switch (args.Unit.ToLowerInvariant())
            {
                case "day":
                    oldestDate = newestDate.Subtract(TimeSpan.FromDays(args.Limit));
                    break;
                case "hour":
                    oldestDate = newestDate.Subtract(TimeSpan.FromHours(args.Limit));
                    break;
                default:
                    oldestDate = newestDate.Subtract(TimeSpan.FromMinutes(args.Limit));
                    break;
            }
        }

        protected void CalculateLinechartSettings(LineChartArgs args,
            GeckoLineChart data, 
            GeckoValues yAxis, 
            List<DateTime> xAxis)
        {

            var minY = args.ScaleY
                           ? Scale(yAxis.Min(), args.ScaleUp, args.ScaleDown, args.ScaleYDecimalPlaces)
                           : Scale(yAxis.Min(), 1, 1, args.ScaleYDecimalPlaces);
            var midY = args.ScaleY
                           ? Scale(yAxis.Mid(), args.ScaleUp, args.ScaleDown, args.ScaleYDecimalPlaces)
                           : Scale(yAxis.Mid(), 1, 1, args.ScaleYDecimalPlaces);
            var maxY = args.ScaleY
                           ? Scale(yAxis.Max(), args.ScaleUp, args.ScaleDown, args.ScaleYDecimalPlaces)
                           : Scale(yAxis.Max(), 1, 1, args.ScaleYDecimalPlaces);

            var colourKey = string.Format("linechart.{0}.{1}", args.Site, args.Check);
            data.Settings.Colour = myColourPicker.Next(colourKey).ToString();
            data.Settings.Y.Add(minY.ToString());
            data.Settings.Y.Add(midY.ToString());
            data.Settings.Y.Add(maxY.ToString());

            var minDate = xAxis.Min();
            var maxDate = xAxis.Max();

            if (maxDate.Subtract(minDate).TotalDays < 1)
            {
                data.Settings.X.Add(minDate.ToShortTimeString());
                data.Settings.X.Add(minDate.AddMinutes(
                    maxDate.Subtract(minDate).TotalMinutes / 2).ToShortTimeString());
                data.Settings.X.Add(maxDate.ToShortTimeString());
            }
            else
            {
                var midMins = maxDate.Subtract(minDate).TotalMinutes / 2;
                var midDate = maxDate.Subtract(TimeSpan.FromMinutes(midMins));

                
                data.Settings.X.Add(minDate.ToString(Linechart.X_Axis_DateFormat));
                data.Settings.X.Add(midDate.ToString(Linechart.X_Axis_DateFormat));
                data.Settings.X.Add(maxDate.ToString(Linechart.X_Axis_DateFormat));
            }           
        }
    }
}