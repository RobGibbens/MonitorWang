
using MonitorWang.Core;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;
using MonitorWang.Tests.Mocks;
using NUnit.Framework;

namespace MonitorWang.Tests.Bdd
{
    public abstract class HealthCheckDomain : BddTestDomain
    {
        protected IMockMessenger MessengerMock { get; set; }
        protected IHealthCheckPlugin HealthCheck { get; set; }

        protected HealthCheckDomain()
        {
            MessengerMock = new MockMessenger();
            Messenger.Initialise(MessengerMock);
        }

        public virtual void TheHealthCheckIsInvoked()
        {
            SafeExecute(() => HealthCheck.Execute());
        }

        public virtual void TheMessageShouldIndicateSuccess()
        {
            TheMessageAtPosition_ShouldHaveA_Result(0, true);
        }

        public virtual void TheMessageShouldIndicateFailure()
        {
            TheMessageAtPosition_ShouldHaveA_Result(0, false);
        }

        public virtual void TheMessageAtPosition_ShouldHaveA_Result(int index, bool expectedResult)
        {
            var msg = (HealthCheckData)MessengerMock.Sent[index];
            Assert.That(msg.Result == expectedResult, Is.True);
        }

        public virtual void ShouldHavePublished_Messages(int expected)
        {
            Assert.That(MessengerMock.Sent.Count, Is.EqualTo(expected));
        }

        public override void Dispose()
        {
            
        }
    }
}