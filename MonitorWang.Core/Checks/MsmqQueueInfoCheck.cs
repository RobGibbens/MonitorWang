using System;
using System.Messaging;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Checks
{
    public class MsmqQueueInfoCheckConfig : PluginConfigBase
    {
        public string QueueName { get; set; }
    }

    public class MsmqQueueInfoCheck : IHealthCheckPlugin
    {
        protected readonly MsmqQueueInfoCheckConfig myConfig;
        protected PluginDescriptor myIdentity;
       
        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="config"></param>
        public MsmqQueueInfoCheck(MsmqQueueInfoCheckConfig config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
            {
                Description = string.Format("Gathers information about the queue '{0}'", config.QueueName),
                TypeId = new Guid("7C6F11A6-7265-4aee-AB23-BA549EACB592"),
                Name = myConfig.FriendlyId
            };
        }

        public Status Status { get; set; }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }

        public virtual void Execute()
        {
            Messenger.Publish(DoTest());
        }

        public void Initialise()
        {
            
        }

        protected virtual HealthCheckData DoTest()
        {
            // check it even exists!
            var count = 0;
            DateTime? oldestMessageDated = null;
            var exists = MessageQueue.Exists(myConfig.QueueName);
            string info;

            if (exists)
            {
                var queue = new MessageQueue(myConfig.QueueName)
                {
                    MessageReadPropertyFilter = new MessagePropertyFilter
                    {
                        ArrivedTime = true
                    }
                };

                var me = queue.GetMessageEnumerator2();
                var timeout = new TimeSpan(0, 0, 0);

                while (me.MoveNext(timeout))
                {
                    var msg = me.Current;

                    if (!oldestMessageDated.HasValue)
                        oldestMessageDated = msg.ArrivedTime;
                    else if (msg.ArrivedTime < oldestMessageDated)
                        oldestMessageDated = msg.ArrivedTime;

                    count++;
                }

                info = string.Format("Queue {0} has {1} messages", myConfig.QueueName, count);
            }
            else
            {
                info = string.Format("Queue {0} does not exist!", myConfig.QueueName);
            }

            var props = new ResultProperties
                            {
                                {"Queue", myConfig.QueueName},
                                {"Count", count.ToString()}
                            };

            if (oldestMessageDated.HasValue)
                props.Add("Oldest", oldestMessageDated.ToString());

            var result = new HealthCheckData
            {
                Identity = Identity,
                Info = info,
                Result = exists,
                Properties = props
            };

            if (exists)
                result.ResultCount = count;

            return result;
        }
    }
}