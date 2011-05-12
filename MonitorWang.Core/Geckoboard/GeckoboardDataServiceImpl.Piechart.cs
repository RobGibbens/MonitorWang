using System;
using MonitorWang.Core.Geckoboard.DataProvider;
using MonitorWang.Core.Geckoboard.Entities;
using System.Linq;

namespace MonitorWang.Core.Geckoboard
{
    public partial class GeckoboardDataServiceImpl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual GeckoPieChart GetGeckoboardPieChartForAllSites()
        {
            var data = new GeckoPieChart();

            try
            {
                var rawData = myDataProvider.GetPieChartDataForAllSites();

                var pieces = from part in rawData
                             select new GeckoPieChart.Piece
                                        {
                                            Label = string.Format("{0} ({1})",
                                                                  part.Label, part.Count),
                                            Value = part.Count,
                                            Colour = myColourPicker.Next(part.Label).ToString()
                                        };

                data.Item = pieces.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(Logger.Event.During("GetGeckoboardPieChartForAllSites")
                    .Encountered(ex));
                throw;
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual GeckoPieChart GetGeckoboardPieChartForSite(string site)
        {
            var data = new GeckoPieChart();

            try
            {
                var rawData = myDataProvider.GetPieChartDataForSite(site);

                var pieces = from part in rawData
                             select new GeckoPieChart.Piece
                             {
                                 Label = string.Format("{0} ({1})",
                                                       part.Label, part.Count),
                                 Value = part.Count,
                                 Colour = myColourPicker.Next(part.Label).ToString()
                             };

                data.Item = pieces.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(Logger.Event.During("GetGeckoboardPieChartForSite")
                    .Encountered(ex));
                throw;
            }

            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GeckoPieChart GetGeckoboardPieChartForCheck(PieChartArgs args)
        {
            var data = new GeckoPieChart();

            try
            {
                var rawData = myDataProvider.GetGeckoboardPieChartForCheck(args);

                var pieces = from part in rawData
                             select new GeckoPieChart.Piece
                             {
                                 Label = string.Format("{0} ({1})",
                                                       part.Label, part.Count),
                                 Value = part.Count,
                                 Colour = myColourPicker.Next(part.Label).ToString()
                             };

                data.Item = pieces.ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(Logger.Event.During("GetGeckoboardPieChartForCheck")
                    .Encountered(ex));
                throw;
            }

            return data;
        }
    }
}