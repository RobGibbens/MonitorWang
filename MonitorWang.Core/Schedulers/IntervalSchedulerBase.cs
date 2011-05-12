using System;
using System.Threading;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Schedulers
{
    /// <summary>
    /// Provides a simple interval base scheduler for anything that 
    /// wants to plug into a windows service
    /// </summary>
    public abstract class IntervalSchedulerBase : IServicePlugin
    {
        protected readonly EventWaitHandle myWaitHandle;
        protected readonly TimeSpan myInterval;

        protected Thread myWorkerThread;
        protected bool myIsPaused;
        protected bool myIsJustStarted;
        protected IntervalSchedulerConfig myConfig;

        protected IntervalSchedulerBase(IntervalSchedulerConfig config)
        {
            myConfig = config;
            myIsJustStarted = true;
            myInterval = TimeSpan.FromSeconds(config.IntervalInSeconds);
            myWaitHandle = new AutoResetEvent(false);
        }
       
        public abstract PluginDescriptor Identity { get; }
        protected abstract void Execute();
        
        public void Start()
        {
            myWorkerThread = new Thread(StartIntervalTask);
            myWorkerThread.Start();
        }

        protected virtual void StartIntervalTask()
        {
            while (true)
            {
                if (!myIsJustStarted)
                {
                    if (myWaitHandle.WaitOne(myInterval))
                        break;
                }
                else
                {
                    myIsJustStarted = false;
                }

                if (myIsPaused)
                    continue;

                Execute();
            }
        }

        public void Stop()
        {
            // are we trying to stop it before its started?
            if (myWorkerThread == null)
                return;

            myWaitHandle.Set();
            myWorkerThread.Join();
        }

        public void Pause()
        {
            myIsPaused = true;
        }

        public void Continue()
        {
            myIsPaused = false;
        }

        public Status Status { get; set; }

        public virtual void Initialise()
        {
            // do nothing
        }
    }
}