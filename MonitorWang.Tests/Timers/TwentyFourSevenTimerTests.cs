using System;
using System.Collections.Generic;
using System.Threading;
using MonitorWang.Core.Schedulers;
using NUnit.Framework;
using System.Linq;

namespace MonitorWang.Tests.Timers
{
    [TestFixture]
    public class WhenDuplicateTimesAreAddedToTheSameDay
    {
        protected TwentyFourSevenTimerConfig myConfig;
        protected TwentyFourSevenTimer myTimer;

        [TestFixtureSetUp]
        public void Given()
        {
            myConfig = new TwentyFourSevenTimerConfig
                           {
                               Monday = "12:34:56,12:34:56,00:00:01"                               
                           };
            myTimer = new TwentyFourSevenTimer(myConfig);
        }

        [Test]
        public void ThenTheDuplicatesShouldBeRemoved()
        {
            Assert.That(myTimer.Alarms[DayOfWeek.Monday].Count, Is.EqualTo(2));
        }

        [Test]
        public void ThenTheDayShouldContainTheCorrectAlarms()
        {
            var expected = new TimeSpan(12, 34, 56);
            Assert.That(myTimer.Alarms[DayOfWeek.Monday].Contains(expected), Is.True);
            expected = new TimeSpan(00, 00, 01);
            Assert.That(myTimer.Alarms[DayOfWeek.Monday].Contains(expected), Is.True);
        }
    }
    
    [TestFixture]
    public class WhenSettingWeekendOnlyAlarms
    {
        protected TwentyFourSevenTimerConfig myConfig;
        protected TwentyFourSevenTimer myTimer;

        [TestFixtureSetUp]
        public void Given()
        {
            myConfig = new TwentyFourSevenTimerConfig
                           {
                               Weekend = "12:34:56,00:00:01"
                           };
            myTimer = new TwentyFourSevenTimer(myConfig);
        }

        [Test]
        public void ThenSaturdayAndSundayShouldHaveAlarmsSet()
        {
            Assert.That(myTimer.Alarms[DayOfWeek.Saturday].Count, Is.GreaterThan(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Sunday].Count, Is.GreaterThan(0));
        }

        [Test]
        public void ThenNoWeekdaysShouldHaveAlarmsSet()
        {
            Assert.That(myTimer.Alarms[DayOfWeek.Monday].Count, Is.EqualTo(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Tuesday].Count, Is.EqualTo(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Wednesday].Count, Is.EqualTo(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Thursday].Count, Is.EqualTo(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Friday].Count, Is.EqualTo(0));
        }
    }
    
    [TestFixture]
    public class WhenSettingWeekdayOnlyAlarms
    {
        protected TwentyFourSevenTimerConfig myConfig;
        protected TwentyFourSevenTimer myTimer;

        [TestFixtureSetUp]
        public void Given()
        {
            myConfig = new TwentyFourSevenTimerConfig
                           {
                               Weekdays = "12:34:56,00:00:01"
                           };
            myTimer = new TwentyFourSevenTimer(myConfig);
        }

        [Test]
        public void ThenSaturdayAndSundayShouldNotHaveAlarmsSet()
        {
            Assert.That(myTimer.Alarms[DayOfWeek.Saturday].Count, Is.EqualTo(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Sunday].Count, Is.EqualTo(0));
        }

        [Test]
        public void ThenAllWeekdaysShouldHaveAlarmsSet()
        {
            Assert.That(myTimer.Alarms[DayOfWeek.Monday].Count, Is.GreaterThan(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Tuesday].Count, Is.GreaterThan(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Wednesday].Count, Is.GreaterThan(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Thursday].Count, Is.GreaterThan(0));
            Assert.That(myTimer.Alarms[DayOfWeek.Friday].Count, Is.GreaterThan(0));
        }
    }
    
    [TestFixture]
    public class WhenMixingWeekendAndWeekendDayAlarms
    {
        protected TwentyFourSevenTimerConfig myConfig;
        protected TwentyFourSevenTimer myTimer;

        [TestFixtureSetUp]
        public void Given()
        {
            myConfig = new TwentyFourSevenTimerConfig
                           {
                               Weekend = "12:34:56,00:00:01",
                               Saturday = "00:00:02",
                               Sunday = "00:00:03"
                           };
            myTimer = new TwentyFourSevenTimer(myConfig);
        }

        [Test]
        public void ThenTheNumberOfAlarmsSetShouldBeCorrect()
        {
            Assert.That(myTimer.Alarms[DayOfWeek.Saturday].Count, Is.EqualTo(3));
            Assert.That(myTimer.Alarms[DayOfWeek.Sunday].Count, Is.EqualTo(3));
        }

        [Test]
        public void ThenSaturdayAndSundayShouldHaveTheWeekendAlarmsSet()
        {
            var expected = new TimeSpan(12, 34, 56);
            Assert.That(myTimer.Alarms[DayOfWeek.Saturday].Contains(expected), Is.True);
            Assert.That(myTimer.Alarms[DayOfWeek.Sunday].Contains(expected), Is.True);
            expected = new TimeSpan(00, 00, 01);
            Assert.That(myTimer.Alarms[DayOfWeek.Saturday].Contains(expected), Is.True);
            Assert.That(myTimer.Alarms[DayOfWeek.Sunday].Contains(expected), Is.True);
        }

        [Test]
        public void ThenSaturdayShouldHaveItsDayAlarmSet()
        {
            var expected = new TimeSpan(00, 00, 02);
            Assert.That(myTimer.Alarms[DayOfWeek.Saturday].Contains(expected), Is.True);
        }

        [Test]
        public void ThenSundayShouldHaveItsDayAlarmSet()
        {
            var expected = new TimeSpan(00, 00, 03);
            Assert.That(myTimer.Alarms[DayOfWeek.Sunday].Contains(expected), Is.True);
        }
    }

    [TestFixture]
    public class WhenTestingA5SecondAlarmRange
    {
        protected TwentyFourSevenTimerConfig myConfig;
        protected TwentyFourSevenTimer myTimer;
        protected IEnumerable<DateTime> myTriggeredAlarms;

        [TestFixtureSetUp]
        public void Given()
        {
            myConfig = new TwentyFourSevenTimerConfig
            {
                Everyday = "00:00:01,00:00:02,00:00:03,00:00:04,00:00:05,00:01:01,00:02:01"
            };
            myTimer = new TwentyFourSevenTimer(myConfig, 
                new FakeNow(DateTime.Now.SetTimeOfDay(0,0,2)));
            Thread.Sleep(5000);
            myTriggeredAlarms = myTimer.Triggered();
        }

        [Test]
        public void ThenTheNumberOfAlarmsTriggeredShouldBeCorrect()
        {
            Assert.That(myTriggeredAlarms.Count(), Is.EqualTo(3));
        }

        [Test]
        public void ThenTheLastAlarmTriggeredShouldBeCorrect()
        {
            var expected = DateTime.Now.SetTimeOfDay(0, 0, 5);
            Assert.That(myTimer.LastAlarmTriggered, Is.EqualTo(expected));
        }

        [Test]
        public void ThenTheTriggeredAlarmsAreTheCorrectOnes()
        {
            var expected = DateTime.Now.SetTimeOfDay(0, 0, 3);
            Assert.That(myTriggeredAlarms.Contains(expected), Is.True);
            expected = DateTime.Now.SetTimeOfDay(0, 0, 4);
            Assert.That(myTriggeredAlarms.Contains(expected), Is.True);
            expected = DateTime.Now.SetTimeOfDay(0, 0, 5);
            Assert.That(myTriggeredAlarms.Contains(expected), Is.True);            
        }
    }

}