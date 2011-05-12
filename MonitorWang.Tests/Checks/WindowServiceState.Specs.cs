using System;
using System.Collections.Generic;
using MonitorWang.Core.Checks;
using NUnit.Framework;
using StoryQ;

namespace MonitorWang.Tests.Checks
{
    [TestFixture]
    public class WindowServiceStateSpecs
    {
        [Test]
        public void ValidServiceCorrectState()
        {
            using (var domain = new WindowServiceStateDomain())
            {
                new Story("The ensure the HealthCheck behaves as expected")
                    .Tag("HealthCheck")
                    .InOrderTo("Ensure no failure notification message is published")
                    .AsA("MonitorWang user")
                    .IWant("The correct behaviour")
                    .WithScenario("A valid windows service name specified and it should be in the expected state")
                    .Given(domain.TheCheckComponent, new WindowsServiceStateCheckConfig
                                                  {
                                                      Enabled = true,
                                                      ExpectedState = "Running",
                                                      FriendlyId = "HealthCheckTest",
                                                      Services = new List<string>
                                                                     {
                                                                         "DHCP Client"
                                                                     }
                                                  })
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ThrewNoException)
                    .And(domain.ShouldHavePublished_Messages, 0)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void ValidServiceIncorrectState()
        {
            using (var domain = new WindowServiceStateDomain())
            {
                new Story("The ensure the HealthCheck behaves as expected")
                    .Tag("HealthCheck")
                    .InOrderTo("Ensure a failure message is published")
                    .AsA("MonitorWang user")
                    .IWant("The correct behaviour")
                    .WithScenario("A valid windows service name specified but the expected state is incorrect")
                    .Given(domain.TheCheckComponent, new WindowsServiceStateCheckConfig
                                                  {
                                                      Enabled = true,
                                                      ExpectedState = "Stopped",
                                                      FriendlyId = "HealthCheckTest",
                                                      Services = new List<string>
                                                                     {
                                                                         "DHCP Client"
                                                                     }
                                                  })
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ShouldHavePublished_Messages, 1)
                    .And(domain.TheMessageShouldIndicateFailure)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void InvalidService()
        {
            const string serviceName = "ASDEDFG123234fwerw4f";

            using (var domain = new WindowServiceStateDomain())
            {
                new Story("The ensure the HealthCheck behaves as expected")
                    .Tag("HealthCheck")
                    .InOrderTo("Ensure an exception is raised")
                    .AsA("MonitorWang user")
                    .IWant("The correct behaviour")
                    .WithScenario("An invalid windows service name specified")
                    .Given(domain.TheCheckComponent, new WindowsServiceStateCheckConfig
                                                  {
                                                      Enabled = true,
                                                      ExpectedState = "Running",
                                                      FriendlyId = "HealthCheckTest",
                                                      Services = new List<string>
                                                                     {
                                                                         serviceName
                                                                     }
                                                  })
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ShouldThrow_Exception, typeof(InvalidOperationException))
                    .And(domain._ShouldBeInTheExceptionMesssage, serviceName)
                    .ExecuteWithReport();
            }
        }

        [Test]
        public void MultipleInvalidServices()
        {
            const string serviceName1 = "ASDEDFG123234fwerw4f";
            const string serviceName2 = "ERseSfsdfDF23£23eE";

            using (var domain = new WindowServiceStateDomain())
            {
                new Story("The ensure the HealthCheck behaves as expected")
                    .Tag("HealthCheck")
                    .InOrderTo("Ensure an exception is raised")
                    .AsA("MonitorWang user")
                    .IWant("The correct behaviour")
                    .WithScenario("Multiple invalid windows service names supplied")
                    .Given(domain.TheCheckComponent, new WindowsServiceStateCheckConfig
                                                  {
                                                      Enabled = true,
                                                      ExpectedState = "Running",
                                                      FriendlyId = "HealthCheckTest",
                                                      Services = new List<string>
                                                                     {
                                                                         serviceName1,
                                                                         serviceName2
                                                                     }
                                                  })
                    .When(domain.TheHealthCheckIsInvoked)
                    .Then(domain.ShouldThrow_Exception, typeof(InvalidOperationException))
                    .And(domain._ShouldBeInTheExceptionMesssage, serviceName1)
                    .And(domain._ShouldBeInTheExceptionMesssage, serviceName2)
                    .ExecuteWithReport();
            }
        }
    }
}