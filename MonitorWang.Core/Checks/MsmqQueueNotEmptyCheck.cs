using System;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Checks
{
    public class MsmqQueueNotEmptyCheckConfig : MsmqQueueInfoCheckConfig
    {

    }

    /// <summary>
    /// This health check will first call <see cref="MsmqQueueInfoCheck"/> to get the
    /// queue information which includes the count of messages. It then modifies the
    /// result based on this count; if the count is non-zero then the result is set to
    /// "fail". 
    /// </summary>
    /// <remarks>This check is primarily used to test errors queues are empty as any message
    /// in one of these queues indicates an exception was encountered handling it</remarks>
    public class MsmqQueueNotEmptyCheck : MsmqQueueInfoCheck
    {
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="config"></param>
        public MsmqQueueNotEmptyCheck(MsmqQueueNotEmptyCheckConfig config)
            : base(config)
        {
            myIdentity = new PluginDescriptor
            {
                Description = string.Format("Checking queue '{0}' is empty", config.QueueName),
                TypeId = new Guid("7C6F11A6-7265-4aee-AB23-BA549EACB592"),
                Name = myConfig.FriendlyId
            };
        }


        protected override HealthCheckData DoTest()
        {
            var result = base.DoTest();

            // amend the result based on the queue msg count
            if (result.Result.GetValueOrDefault(false))
            {
                result.Result = (Convert.ToInt32(result.Properties["Count"]) == 0);
                result.Info = string.Format("Queue {0} is {1}", myConfig.QueueName,
                                            result.Result.Value ? "empty" : "not empty");
            }

            return result;
        }
    }
}