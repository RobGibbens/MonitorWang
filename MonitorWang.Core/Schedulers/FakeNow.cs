using System;
using MonitorWang.Core.Interfaces;

namespace MonitorWang.Core.Schedulers
{
    public class RealNow : INow
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }

        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
    
    public class FakeNow : INow
    {
        protected readonly DateTime myBaseline;
        protected readonly DateTime myStartTime;

        public FakeNow(DateTime baseline)
        {
            myBaseline = baseline;
            myStartTime = DateTime.Now;
        }

        public DateTime Now()
        {
            return myBaseline + (DateTime.Now - myStartTime);

        }

        public DateTime UtcNow()
        {
            return Now().ToUniversalTime();
        }
    }
}