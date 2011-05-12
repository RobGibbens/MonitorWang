using Growl.Connector;
using MonitorWang.Core.Growl;
using MonitorWang.Core.Interfaces.Entities;
using NUnit.Framework;

namespace MonitorWang.Tests.Growl
{
    [TestFixture]
    public abstract class WhenUsingTheGrowlCountNotificationFinaliserBase
    {
        protected Notification myNotificationUnderTest;
        protected GrowlCountNotificationFinaliser myFinaliserUnderTest;
        protected Priority myExpectedPriorityWhenLower;
        protected Priority myExpectedPriorityWhenEqual;
        protected Priority myExpectedPriorityWhenHigher;

        public const double PRIORITY_THRESHOLD = 2.34;

        [TestFixtureSetUp]
        public virtual void Given()
        {            
            SetupNotification();
            myExpectedPriorityWhenLower = ExpectedPriorityWhenResultCountIsLower();         
            myExpectedPriorityWhenEqual = ExpectedPriorityWhenResultCountIsEqual();
            myExpectedPriorityWhenHigher = ExpectedPriorityWhenResultCountIsHigher();
            SetupSut();    
        }

        protected virtual GrowlCountNotificationFinaliser SetupSut()
        {
            myFinaliserUnderTest = new GrowlCountNotificationFinaliser
            {
                Check = "*",
                Threshold = PRIORITY_THRESHOLD,
                HigherPriority = myExpectedPriorityWhenHigher.ToString(),
                LowerPriority = myExpectedPriorityWhenLower.ToString()
            };

            return myFinaliserUnderTest;
        }

        protected virtual void SetupNotification()
        {
            myNotificationUnderTest = new Notification("testApp", "testName", "testId", "testTitle", "testText")
            {
                Priority = Priority.Normal
            };
        }

        protected abstract Priority ExpectedPriorityWhenResultCountIsLower();
        protected abstract Priority ExpectedPriorityWhenResultCountIsEqual();
        protected abstract Priority ExpectedPriorityWhenResultCountIsHigher();

        [Test]
        public virtual void ThenThePriorityShouldBeCorrectForALowerResultCount()
        {
            SetupNotification();
            var result = new HealthCheckResult
                             {
                                 Check = new HealthCheckData
                                             {
                                                ResultCount = PRIORITY_THRESHOLD - 1              
                                             }
                             };
            
            myFinaliserUnderTest.Finalise(result, myNotificationUnderTest);
            Assert.That(myNotificationUnderTest.Priority == myExpectedPriorityWhenLower, Is.True, "Expected {0}, Actual {1}",
                myExpectedPriorityWhenLower, myNotificationUnderTest.Priority);
        }

        [Test]
        public void ThenThePriorityShouldBeCorrectForAnEqualResultCount()
        {
            SetupNotification();
            var result = new HealthCheckResult
                             {
                                 Check = new HealthCheckData
                                             {
                                                ResultCount = PRIORITY_THRESHOLD
                                             }
                             };
            
            myFinaliserUnderTest.Finalise(result, myNotificationUnderTest);
            Assert.That(myNotificationUnderTest.Priority == myExpectedPriorityWhenEqual, Is.True, "Expected {0}, Actual {1}",
                myExpectedPriorityWhenEqual, myNotificationUnderTest.Priority);
        }

        [Test]
        public void ThenThePriorityShouldBeCorrectForAHigherResultCount()
        {
            SetupNotification();
            var result = new HealthCheckResult
                             {
                                 Check = new HealthCheckData
                                             {
                                                ResultCount = PRIORITY_THRESHOLD + 1
                                             }
                             };
            
            myFinaliserUnderTest.Finalise(result, myNotificationUnderTest);
            Assert.That(myNotificationUnderTest.Priority == myExpectedPriorityWhenHigher, Is.True, "Expected {0}, Actual {1}",
                myExpectedPriorityWhenHigher, myNotificationUnderTest.Priority);
        }
    }

    [TestFixture]
    public class WhenNoPrioritiesHaveBeenSet : WhenUsingTheGrowlCountNotificationFinaliserBase
    {
        protected override Priority ExpectedPriorityWhenResultCountIsLower()
        {
            return Priority.Normal;
        }

        protected override Priority ExpectedPriorityWhenResultCountIsEqual()
        {
            return Priority.Normal;
        }

        protected override Priority ExpectedPriorityWhenResultCountIsHigher()
        {
            return Priority.Normal;
        }
    }

    [TestFixture]
    public class WhenSettingOnlyTheHigherPriority : WhenUsingTheGrowlCountNotificationFinaliserBase
    {
        protected override Priority ExpectedPriorityWhenResultCountIsLower()
        {
            return Priority.Normal;
        }

        protected override Priority ExpectedPriorityWhenResultCountIsEqual()
        {
            return Priority.Emergency;
        }

        protected override Priority ExpectedPriorityWhenResultCountIsHigher()
        {
            return Priority.Emergency;
        }
    }

    [TestFixture]
    public class WhenSettingOnlyTheLowerPriority : WhenUsingTheGrowlCountNotificationFinaliserBase
    {
        protected override Priority ExpectedPriorityWhenResultCountIsLower()
        {
            return Priority.Emergency;
        }

        protected override Priority ExpectedPriorityWhenResultCountIsEqual()
        {
            return Priority.Normal;
        }

        protected override Priority ExpectedPriorityWhenResultCountIsHigher()
        {
            return Priority.Normal;
        }
    }

    [TestFixture]
    public class WhenSettingTheHigherAndLowerPriority : WhenUsingTheGrowlCountNotificationFinaliserBase
    {
        protected override Priority ExpectedPriorityWhenResultCountIsLower()
        {
            return Priority.Moderate;
        }

        protected override Priority ExpectedPriorityWhenResultCountIsEqual()
        {
            return Priority.High;
        }

        protected override Priority ExpectedPriorityWhenResultCountIsHigher()
        {
            return Priority.High;
        }
    }
}