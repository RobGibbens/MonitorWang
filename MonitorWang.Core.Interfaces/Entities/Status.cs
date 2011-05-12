
namespace MonitorWang.Core.Interfaces.Entities
{
    public class Status
    {
        /// <summary>
        /// What was going on?
        /// </summary>
        public string Activity { get; set; }
        /// <summary>
        /// What is the state of this activity
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// General message about the state
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// An id that can be used to correlate this state snapshot
        /// with other data (eg: an exception trace)
        /// </summary>
        public string CorrelationId { get; set; }


        public static Status For(string activity)
        {
            return new Status
                       {
                           Activity = activity
                       };
        }

        public Status StateIsPending()
        {
            return StateIs("Pending");
        }

        public Status StateIsFailure()
        {
            return StateIs("Failure");
        }

        public Status StateIsSuccess()
        {
            return StateIs("Success");
        }

        public Status StateIs(string state)
        {
            State = state;
            return this;
        }

        public Status Description(string format, params object[] args)
        {
            Message = string.Format(format, args);
            return this;
        }

        public Status CorrelateTo(string id)
        {
            CorrelationId = id;
            return this;
        }

        /// <summary>
        /// Determines the health by comparing the state property
        /// to "Failure" - if it matches it deems the state "unhealthy"
        /// </summary>
        /// <returns></returns>
        public bool IsHealthy()
        {
            return (string.Compare(State, "Failure", true) != 0);
        }
    }
}