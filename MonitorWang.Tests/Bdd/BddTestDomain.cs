using System;
using System.Threading;
using NUnit.Framework;

namespace MonitorWang.Tests.Bdd
{
    public abstract class BddTestDomain : IDisposable
    {
        protected Exception myExpectedException;

        public abstract void Dispose();

        public void SafeExecute(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                myExpectedException = ex;
            }
        }

        public void SimulateBackgroundEvent(Action action, int interval, Action asyncAction)
        {
            var bgThread = new Thread((() =>
            {
                Thread.Sleep(interval);
                asyncAction.Invoke();
            }));
            bgThread.Start();
            action.Invoke();
        }

        public void ThrewNoException()
        {
            Assert.Null(myExpectedException);
        }

        public void ShouldThrow_Exception(Type expected)
        {
            Assert.That(myExpectedException, Is.Not.Null);
            Assert.That(myExpectedException, Is.TypeOf(expected));
        }

        public void _ShouldBeInTheExceptionMesssage(string content)
        {
            Assert.That(myExpectedException.Message, Is.StringContaining(content));
        }
    }
}