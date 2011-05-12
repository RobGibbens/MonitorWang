using System;
using Growl.Connector;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Growl
{
    /// <summary>
    /// This finaliser will alter the Growl Notification Priority based on the result state.
    /// You can tune the Priority for the success and failure state of a result.
    /// </summary>
    public class GrowlSuccessNotificationFinaliser : GrowlNotificationInterceptorBase
    {
        /// <summary>
        /// The priorty to set if the result is "success"
        /// </summary>
        public string OnSuccess { get; set; }
        /// <summary>
        /// If you want the notification to stay put on 
        /// screen then set this to true
        /// </summary>
        public bool SuccessIsSticky { get; set; }
        /// <summary>
        /// The text to override the default
        /// </summary>
        public string SuccessMessage { get; set; }
        /// <summary>
        /// The priority to set if the result is "failure"
        /// </summary>
        public string OnFailure { get; set; }
        /// <summary>
        /// If you want the notification to stay put on 
        /// screen then set this to true
        /// </summary>
        public bool FailureIsSticky { get; set; }
        /// <summary>
        /// The text to override the default
        /// </summary>
        public string FailureMessage { get; set; }

        /// <summary>
        /// This method is used to adjust the Growl <paramref name="notification"/>. The 
        /// <paramref name="result"/> can be inspected and notification properties set
        /// according to custom logic.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="notification"></param>
        public override void Finalise(HealthCheckResult result, Notification notification)
        {
            if (!result.Check.Result.HasValue)
                return;
            if (result.Check.Result.Value)
            {
                if (!string.IsNullOrEmpty(OnSuccess))
                    notification.Priority = (Priority)Enum.Parse(typeof(Priority), OnSuccess, true);
                if (!string.IsNullOrEmpty(SuccessMessage))
                    notification.Text = SuccessMessage;
                notification.Sticky = SuccessIsSticky;
            }
            else
            {
                if (!string.IsNullOrEmpty(OnFailure))
                    notification.Priority = (Priority)Enum.Parse(typeof(Priority), OnFailure, true);
                if (!string.IsNullOrEmpty(FailureMessage))
                    notification.Text = FailureMessage;
                notification.Sticky = FailureIsSticky;
            }
        }
    }

    /// <summary>
    /// This finaliser will set the Growl Notification Priority based on the Result.ResultCount property
    /// value. You can preset the notification priority should the result count value be lower or higher
    /// than the threshold value you also set
    /// </summary>
    public class GrowlCountNotificationFinaliser : GrowlNotificationInterceptorBase
    {
        /// <summary>
        /// Set this number to split the priority - Higher and Lower
        /// </summary>
        public double Threshold { get; set; }
        /// <summary>
        /// The priority to set when the result count is higher or
        /// equal to the <see cref="Threshold"/> value
        /// </summary>
        public string HigherPriority { get; set; }
        /// <summary>
        /// The text to replace the default
        /// </summary>
        public string HigherMessage { get; set; }
        /// <summary>
        /// If you want the notification to stay put on 
        /// screen then set this to true
        /// </summary>
        public bool HigherIsSticky { get; set; }
        /// <summary>
        /// The priority to set when the result count is lower
        /// than the <see cref="Threshold"/> value
        /// </summary>
        public string LowerPriority { get; set; }
        /// <summary>
        /// The text to replace the default
        /// </summary>
        public string LowerMessage { get; set; }
        /// <summary>
        /// If you want the notification to stay put on 
        /// screen then set this to true
        /// </summary>
        public bool LowerIsSticky { get; set; }

        public override void Finalise(HealthCheckResult result, Notification notification)
        {
            if (!result.Check.ResultCount.HasValue)
                return;

            var rc = result.Check.ResultCount;

            if (rc >= Threshold)
            {
                if (!string.IsNullOrEmpty(HigherPriority))
                    notification.Priority = (Priority)Enum.Parse(typeof(Priority), HigherPriority, true);
                if (!string.IsNullOrEmpty(HigherMessage))
                    notification.Text = HigherMessage;
                notification.Sticky = HigherIsSticky;
            }
            else
            {
                if (!string.IsNullOrEmpty(LowerPriority))
                    notification.Priority = (Priority)Enum.Parse(typeof(Priority), LowerPriority, true);
                if (!string.IsNullOrEmpty(LowerMessage))
                    notification.Text = LowerMessage;
                notification.Sticky = LowerIsSticky;
            }
        }
    }
}