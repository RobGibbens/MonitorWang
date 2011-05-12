using System;

namespace MonitorWang.Core.Interfaces.Entities
{
    public class CriticalFailureDetails
    {
        /// <summary>
        /// This is the unique id of this failure incident. It should be
        /// used to correlate this back to the Logger.Error() message that
        /// would also have recorded this failure.
        /// </summary>
        public Guid Id { get; set; }
    }
}