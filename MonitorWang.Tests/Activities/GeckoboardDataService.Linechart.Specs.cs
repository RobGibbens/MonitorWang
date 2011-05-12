
using System;
using System.Collections.Generic;
using MonitorWang.Core.Geckoboard;
using MonitorWang.Core.Geckoboard.DataProvider;
using MonitorWang.Tests.Bdd;
using NUnit.Framework;
using StoryQ;

namespace MonitorWang.Tests.Activities
{
    [TestFixture]
    public class GeckoboardDataServiceLinechartSpecs : BddFeature
    {
        protected override Feature DescribeFeature()
        {
            return new Story("Displaying a linechart in Geckoboard")
                .InOrderTo("Render a linechart")
                .AsA("Geckoboard user")
                .IWant("The correct Geckoboard Linechart data");
        }

        [Test]
        public void LinechartDayCheckRateNoData()
        {
            var args = new LineChartArgs
                           {
                               MaxItems = 20,
                               Limit = 10,
                               Check = "Test",
                               Unit = "day",
                               DataOperation = DataOperationType.Count,
                               Outcome = OutcomeType.Any,
                               DecimalPlaces = 0,
                               EndDate = new DateTime(2011, 04, 11, 12, 0, 0)
                           };

            using (var domain = new GeckoboardDataServiceLinechartDomain(args))
            {
                var xMin = args.EndDate.Subtract(TimeSpan.FromDays(args.Limit - 1));
                xMin = new DateTime(xMin.Year, xMin.Month, xMin.Day, 0, 0, 0);
                var xMax = new DateTime(args.EndDate.Year, args.EndDate.Month, args.EndDate.Day, 0, 0, 0);

                Feature.WithScenario("There is no daily monitorwang data for the args specified")
                    .Given(domain.ThereIsNoData)
                    .When(domain.TheLinechartCheckRateMethodIsInvoked)
                    .Then(domain.TheResultIsNotNull)
                        .And(domain.TheResultSettingsIsNotNull)
                        .And(domain.TheResultContains_Items, Math.Min(args.Limit, args.MaxItems))
                        .And(domain.AllTheResultsAreZero)
                        .And(domain.TheSettingsXAxisHasThreeValues)
                        .And(domain.TheSettingsXAxisHas_AsTheMinValue, xMin.ToString(GeckoboardDataServiceImpl.Linechart.X_Axis_DateFormat))
                        .And(domain.TheSettingsXAxisHas_AsTheMaxValue, xMax.ToString(GeckoboardDataServiceImpl.Linechart.X_Axis_DateFormat))
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void LinechartHourCheckRateNoData()
        {
            var args = new LineChartArgs
                           {
                               MaxItems = 10,
                               Limit = 10,
                               Check = "Test",
                               Unit = "hour",
                               DataOperation = DataOperationType.Count,
                               Outcome = OutcomeType.Any,
                               DecimalPlaces = 0,
                               EndDate = new DateTime(2011, 04, 11, 12, 0, 0)
                           };

            using (var domain = new GeckoboardDataServiceLinechartDomain(args))
            {
                Feature.WithScenario("There is no hourly monitorwang data for the args specified")
                    .Given(domain.ThereIsNoData)
                    .When(domain.TheLinechartCheckRateMethodIsInvoked)
                    .Then(domain.TheResultIsNotNull)
                        .And(domain.TheResultSettingsIsNotNull)
                        .And(domain.TheResultContains_Items, Math.Min(args.Limit, args.MaxItems))
                        .And(domain.AllTheResultsAreZero)
                        .And(domain.TheSettingsXAxisHasThreeValues)
                        .And(domain.TheSettingsXAxisHas_AsTheMinValue, "03:00")
                        .And(domain.TheSettingsXAxisHas_AsTheMaxValue, "12:00")
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void LinechartMinuteCheckRateNoData()
        {
            var args = new LineChartArgs
                           {
                               MaxItems = 60,
                               Limit = 60,
                               Check = "Test",
                               Unit = "minute",
                               DataOperation = DataOperationType.Count,
                               Outcome = OutcomeType.Any,
                               DecimalPlaces = 0,
                               EndDate = new DateTime(2011, 04, 11, 12, 0, 0)
                           };

            using (var domain = new GeckoboardDataServiceLinechartDomain(args))
            {
                Feature.WithScenario("There is no minute monitorwang data for the args specified")
                    .Given(domain.ThereIsNoData)
                    .When(domain.TheLinechartCheckRateMethodIsInvoked)
                    .Then(domain.TheResultIsNotNull)
                        .And(domain.TheResultSettingsIsNotNull)
                        .And(domain.TheResultContains_Items, Math.Min(args.Limit, args.MaxItems))
                        .And(domain.AllTheResultsAreZero)
                        .And(domain.TheSettingsXAxisHasThreeValues)
                        .And(domain.TheSettingsXAxisHas_AsTheMinValue, "11:01")
                        .And(domain.TheSettingsXAxisHas_AsTheMaxValue, "12:00")
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void LinechartDayLimitGreaterThanMaxItem()
        {
            var args = new LineChartArgs
                           {
                               MaxItems = 10,
                               Limit = 20,
                               Check = "Test",
                               Unit = "day",
                               DataOperation = DataOperationType.Count,
                               Outcome = OutcomeType.Any,
                               DecimalPlaces = 0,
                               EndDate = new DateTime(2011, 04, 11, 12, 0, 0)
                           };

            using (var domain = new GeckoboardDataServiceLinechartDomain(args))
            {
                var xMin = new DateTime(2011, 03, 24, 0, 0, 0);
                var xMax = new DateTime(args.EndDate.Year, args.EndDate.Month, args.EndDate.Day, 0, 0, 0);

                Feature.WithScenario("There are more items requested than the max allowed")
                    .Given(domain.ThereIsNoData)
                    .When(domain.TheLinechartCheckRateMethodIsInvoked)
                    .Then(domain.TheResultIsNotNull)
                        .And(domain.TheResultSettingsIsNotNull)
                        .And(domain.TheResultContains_Items, Math.Min(args.Limit, args.MaxItems))
                        .And(domain.AllTheResultsAreZero)
                        .And(domain.TheSampleIsEvery_Items, 2)
                        .And(domain.TheSettingsXAxisHasThreeValues)
                        .And(domain.TheSettingsXAxisHas_AsTheMinValue, xMin.ToString(GeckoboardDataServiceImpl.Linechart.X_Axis_DateFormat))
                        .And(domain.TheSettingsXAxisHas_AsTheMaxValue, xMax.ToString(GeckoboardDataServiceImpl.Linechart.X_Axis_DateFormat))
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void LinechartMinuteDataWithinLimitRangeCorrectDecimalPlaces()
        {
            var args = new LineChartArgs
                           {
                               MaxItems = 10,
                               Limit = 10,
                               Check = "Test",
                               Unit = "minute",
                               DataOperation = DataOperationType.Count,
                               Outcome = OutcomeType.Any,
                               DecimalPlaces = 0,
                               EndDate = new DateTime(2011, 04, 11, 12, 0, 0)
                           };

            using (var domain = new GeckoboardDataServiceLinechartDomain(args))
            {
                var data = new List<LineChartData>
                               {
                                   new LineChartData
                                       {
                                           Value = 100,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(2))
                                       },
                                   new LineChartData
                                       {
                                           Value = 200,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(3))
                                       },
                                   new LineChartData
                                       {
                                           Value = 1000,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(5))
                                       },
                               };

                Feature.WithScenario("Minute data is available within the requested range")
                    .Given(domain.ThisDataIsAvailable, data)
                    .When(domain.TheLinechartCheckRateMethodIsInvoked)
                    .Then(domain.TheResultIsNotNull)
                        .And(domain.TheResultSettingsIsNotNull)
                        .And(domain.TheResultContains_Items, Math.Min(args.Limit, args.MaxItems))
                        .And(domain.TheSampleIsEvery_Items, 1)
                        .And(domain.TheSettingsXAxisHasThreeValues)
                        .And(domain.TheSettingsYAxisHasThreeValues)
                        .And(domain.TheSettingsYAxisHas_AsTheMinValue, "0")
                        .And(domain.TheSettingsYAxisHas_AsTheMaxValue, "1000")
                        .And(domain.TheItemAtIndex_Has_Value, 4, "1000")
                        .And(domain.TheItemAtIndex_Has_Value, 6, "200")
                        .And(domain.TheItemAtIndex_Has_Value, 7, "100")
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void LinechartMinuteDataCorrectDecimalPlaces()
        {
            var args = new LineChartArgs
                           {
                               MaxItems = 10,
                               Limit = 10,
                               Check = "Test",
                               Unit = "minute",
                               DataOperation = DataOperationType.Count,
                               Outcome = OutcomeType.Any,
                               DecimalPlaces = 2,
                               EndDate = new DateTime(2011, 04, 11, 12, 0, 0)
                           };

            using (var domain = new GeckoboardDataServiceLinechartDomain(args))
            {
                var data = new List<LineChartData>
                               {
                                   new LineChartData
                                       {
                                           Value = 100.123,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(2))
                                       },
                                   new LineChartData
                                       {
                                           Value = 200.123,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(3))
                                       },
                                   new LineChartData
                                       {
                                           Value = 1000.123,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(5))
                                       },
                               };

                Feature.WithScenario("Data values have the correct number of decimal places")
                    .Given(domain.ThisDataIsAvailable, data)
                    .When(domain.TheLinechartCheckRateMethodIsInvoked)
                    .Then(domain.TheResultIsNotNull)
                        .And(domain.TheResultSettingsIsNotNull)
                        .And(domain.TheResultContains_Items, Math.Min(args.Limit, args.MaxItems))
                        .And(domain.TheSampleIsEvery_Items, 1)
                        .And(domain.TheSettingsXAxisHasThreeValues)
                        .And(domain.TheSettingsYAxisHasThreeValues)
                        .And(domain.TheSettingsYAxisHas_AsTheMinValue, "0")
                        .And(domain.TheSettingsYAxisHas_AsTheMaxValue, "1000")
                        .And(domain.TheItemAtIndex_Has_Value, 4, "1000.12")
                        .And(domain.TheItemAtIndex_Has_Value, 6, "200.12")
                        .And(domain.TheItemAtIndex_Has_Value, 7, "100.12")
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void LinechartMinuteDataOutsideLimitRange()
        {
            var args = new LineChartArgs
                           {
                               MaxItems = 10,
                               Limit = 10,
                               Check = "Test",
                               Unit = "minute",
                               DataOperation = DataOperationType.Count,
                               Outcome = OutcomeType.Any,
                               DecimalPlaces = 2,
                               EndDate = new DateTime(2011, 04, 11, 12, 0, 0)
                           };

            using (var domain = new GeckoboardDataServiceLinechartDomain(args))
            {
                var data = new List<LineChartData>
                               {
                                   new LineChartData
                                       {
                                           Value = 100.123,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(2000))
                                       },
                                   new LineChartData
                                       {
                                           Value = 200.123,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(3000))
                                       },
                                   new LineChartData
                                       {
                                           Value = 1000.123,
                                           When = args.EndDate.Subtract(TimeSpan.FromMinutes(5000))
                                       },
                               };

                Feature.WithScenario("Minute data is available but outside the requested range")
                    .Given(domain.ThisDataIsAvailable, data)
                    .When(domain.TheLinechartCheckRateMethodIsInvoked)
                    .Then(domain.TheResultIsNotNull)
                        .And(domain.TheResultSettingsIsNotNull)
                        .And(domain.TheResultContains_Items, Math.Min(args.Limit, args.MaxItems))
                        .And(domain.TheSampleIsEvery_Items, 1)
                        .And(domain.TheSettingsXAxisHasThreeValues)
                        .And(domain.TheSettingsYAxisHasThreeValues)
                        .And(domain.TheSettingsYAxisHas_AsTheMinValue, "0")
                        .And(domain.TheSettingsYAxisHas_AsTheMaxValue, "0")
                        .And(domain.AllTheResultsAreZero)
                    .ExecuteWithReport();
            }
        }
    }
}