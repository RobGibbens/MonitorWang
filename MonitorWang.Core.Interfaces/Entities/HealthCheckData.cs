using System;

namespace MonitorWang.Core.Interfaces.Entities
{
    public class HealthCheckData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public HealthCheckData()
        {
            GeneratedOnUtc = DateTime.UtcNow;
            CriticalFailure = false;
        }

        /// <summary>
        /// The result of the check if it has a true/false outcome
        /// </summary>
        public bool? Result { get; set; }

        /// <summary>
        /// The count result of the check
        /// </summary>
        public double? ResultCount { get; set; }

        /// <summary>
        /// Indicates a critical failure in the check. The details
        /// about the failure are stored in the <see cref="CriticalFailureDetails"/>
        /// property
        /// </summary>
        public bool CriticalFailure { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CriticalFailureDetails CriticalFailureDetails { get; set; }

        /// <summary>
        /// Identifies the health check
        /// </summary>
        public PluginDescriptor Identity { get; set; }

        /// <summary>
        /// Indicates when the check took place
        /// </summary>
        public DateTime GeneratedOnUtc { get; set; }

        /// <summary>
        /// How long the check took to execute
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// The scheduled this is expected to run again
        /// </summary>
        public DateTime? NextCheckExpected { get; set; }

        /// <summary>
        /// General text field to help describe the result
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// General text field to help categorise the result
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Property bag to contain name/value pairs of result data
        /// </summary>
        public ResultProperties Properties { get; set; }


        public static HealthCheckData For(PluginDescriptor identity,
            string info, params object[] args)
        {
            return new HealthCheckData
                       {
                           Identity = identity,
                           Info = string.Format(info, args)
                       };
        }

        public HealthCheckData Succeeded()
        {
            Result = true;
            return this;
        }

        public HealthCheckData Failed()
        {
            Result = false;
            return this;
        }

        public HealthCheckData AddProperty(string name, string value)
        {
            if (Properties == null)
                Properties = new ResultProperties();

            Properties.Add(name, value);
            return this;
        }

        public HealthCheckData AddTag(string tag)
        {
            return AddTag(tag, ",");
        }

        public HealthCheckData AddTag(string tag, string delimiter)
        {
            if (!string.IsNullOrEmpty(Tags))
                Tags += delimiter;
            Tags += tag;
            return this;
        }
    }
}