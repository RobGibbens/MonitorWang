using System;
using log4net;
using log4net.Config;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core
{
    public class Logger
    {
        public class Event
        {
            public Exception Exception { get; set;}
            public Status Context { get; set; }

            public static Event During(string activity)
            {
                return new Event
                           {                               
                               Context = new Status
                                             {
                                                 Activity = activity,
                                                 CorrelationId = Guid.NewGuid().ToString()
                                             }
                           };
            }

            public Event Encountered(Exception ex)
            {
                Context.StateIsFailure();
                Exception = ex;                
                return this;
            }

            public Event StateIsPending()
            {
                return StateIs("Pending");
            }

            public Event StateIsFailure()
            {
                return StateIs("Failure");
            }

            public Event StateIsSuccess()
            {
                return StateIs("Success");
            }

            public Event StateIs(string state)
            {
                Context.State = state;
                return this;
            }

            public Event Description(string format, params object[] args)
            {
                Context.Message = string.Format(format, args);
                return this;
            }

            public Event CorrelateTo(string id)
            {
                Context.CorrelationId = id;
                return this;
            }
        }


        protected static ILog myLogger;

        static Logger()
        {
            myLogger = LogManager.GetLogger("MonitorWang");
            XmlConfigurator.Configure();
        }

        public static void Debug(string format, params object[] args)
        {
            if (!myLogger.IsDebugEnabled)
                return;

            myLogger.DebugFormat(format, args);
        }

        public static void Error(string format, params object[] args)
        {
            if (!myLogger.IsErrorEnabled)
                return;

            myLogger.ErrorFormat(format, args);
        }

        public static void Error(Event error)
        {
            if (!myLogger.IsErrorEnabled)
                return;
            
            myLogger.ErrorFormat("IncidentId:{0}; Encountered '{1}' during '{2}'; Message:={3}",
                error.Context.CorrelationId,
                error.Exception.GetType().Name,
                error.Context.Activity,
                error.Exception);
        }
    }
}