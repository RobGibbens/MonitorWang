using System;
using System.Collections.Generic;
using MonitorWang.Core.Geckoboard;
using MonitorWang.Core.Geckoboard.DataProvider;
using MonitorWang.Core.Geckoboard.Entities;
using MonitorWang.Tests.Bdd;
using Moq;
using NUnit.Framework;

namespace MonitorWang.Tests.Activities
{
    public class GeckoboardDataServiceLinechartDomain : GeckoboardDataServiceDomain
    {
        protected GeckoLineChart myResult;
        protected LineChartArgs myArgs;

        public GeckoboardDataServiceLinechartDomain()
        {
        }

        public GeckoboardDataServiceLinechartDomain(LineChartArgs args)
            : this()
        {
            myArgs = args;
        }

        public override void Dispose()
        {
            if (myResult == null)
                return;

            if (myResult.Settings.Y.Count == 3)
                Console.WriteLine("Y: Min={0}; Mid={1}; Max={2}",
                    myResult.Settings.Y[0],
                    myResult.Settings.Y[1],
                    myResult.Settings.Y[2]);

            if (myResult.Settings.X.Count == 3)
                Console.WriteLine("X: Min={0}; Mid={1}; Max={2}",
                    myResult.Settings.X[0],
                    myResult.Settings.X[1],
                    myResult.Settings.X[2]);

            for (var i = 0; i < myResult.Item.Count; i++)
            {
                Console.WriteLine("Item[{0}]={1}", i, myResult.Item[i]);
            }            
        }

        public void TheDataServiceActivityComponent()
        {
        }

        public void ThereIsNoData()
        {
            ThisDataIsAvailable(new LineChartData[] {});
        }

        public void ThisDataIsAvailable(IEnumerable<LineChartData> data)
        {
            var mockDp = new Mock<IGeckoboardDataProvider>();
            mockDp.Setup(dp => dp.GetLineChartDataForCheckRate(myArgs)).Returns(data);
            myDataProvider = mockDp.Object;
        }

        public void TheLinechartCheckRateMethodIsInvoked()
        {
            SafeExecute(() =>
                            {
                                var impl = new GeckoboardDataServiceImpl(myDataProvider, myColourPicker);
                                myResult = impl.GetGeckoboardLineChartForCheckRate(myArgs);
                            });
        }

        public void TheResultIsNotNull()
        {
            Assert.That(myResult, Is.Not.Null);
        }
        
        public void TheResultSettingsIsNotNull()
        {
            Assert.That(myResult.Settings, Is.Not.Null);
        }

        public void TheResultContains_Items(long expected)
        {
            Assert.That(myResult.Item.Count, Is.EqualTo(expected));
        }

        public void AllTheResultsAreZero()
        {
            Assert.That(myResult.Item, Is.All.EqualTo("0"));
        }

        public void TheSettingsXAxisHasThreeValues()
        {
            Assert.That(myResult.Settings.X, Is.Not.Null);
            Assert.That(myResult.Settings.X.Count, Is.EqualTo(3));
        }

        public void TheSettingsXAxisHas_AsTheMinValue(string expected)
        {
            Assert.That(myResult.Settings.X, Is.Not.Null);
            Assert.That(myResult.Settings.X[0], Is.EqualTo(expected));
        }

        public void TheSettingsXAxisHas_AsTheMaxValue(string expected)
        {
            Assert.That(myResult.Settings.X, Is.Not.Null);
            Assert.That(myResult.Settings.X[myResult.Settings.X.Count - 1], Is.EqualTo(expected));
        }

        public void TheSampleIsEvery_Items(int expected)
        {
            Assert.That(myArgs.Sample, Is.EqualTo(expected));
        }

        public void TheSettingsYAxisHasThreeValues()
        {
            Assert.That(myResult.Settings.Y, Is.Not.Null);
            Assert.That(myResult.Settings.Y.Count, Is.EqualTo(3));
        }

        public void TheSettingsYAxisHas_AsTheMinValue(string expected)
        {
            Assert.That(myResult.Settings.Y, Is.Not.Null);
            Assert.That(myResult.Settings.Y[0], Is.EqualTo(expected));
        }

        public void TheSettingsYAxisHas_AsTheMaxValue(string expected)
        {
            Assert.That(myResult.Settings.Y, Is.Not.Null);
            Assert.That(myResult.Settings.Y[myResult.Settings.Y.Count - 1], Is.EqualTo(expected));
        }

        public void TheItemAtIndex_Has_Value(int index, string expected)
        {
            Assert.That(myResult.Item[index], Is.EqualTo(expected));
        }
    }
}