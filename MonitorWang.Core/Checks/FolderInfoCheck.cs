using System;
using System.IO;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Checks
{
    public class FolderInfoCheckConfig : PluginConfigBase
    {
        public string FolderLocation { get; set; }
    }

    public class FolderInfoCheck : IHealthCheckPlugin
    {
        protected readonly FolderInfoCheckConfig myConfig;
        protected readonly SmartLocation myFolderLocation;
        protected PluginDescriptor  myIdentity;

        /// <summary>
        /// default ctor
        /// </summary>
        public FolderInfoCheck(FolderInfoCheckConfig config)
        {
            myConfig = config;
            myFolderLocation = new SmartLocation(config.FolderLocation);
            myIdentity = new PluginDescriptor
            {
                Description = string.Format("Reports information about folder {0}", myFolderLocation.Location),
                TypeId = new Guid("5A3F5E7D-28B6-4ce9-A77A-66E83FEB888A"),
                Name = myConfig.FriendlyId
            };
        }

        public Status Status { get; set; }

        public PluginDescriptor Identity
        {
            get { return myIdentity; }
        }
       
        public void Initialise()
        {
            // do nothing
        }

        public virtual void Execute()
        {
            var fi = new DirectoryInfo(myFolderLocation.Location);
            var result = new HealthCheckData
                             {
                                 Identity = Identity,
                                 Info = string.Format("Information about folder '{0}'...", myFolderLocation.Location),
                                 Result = fi.Exists
                             };

            if (result.Result.GetValueOrDefault(false))
            {
                result.ResultCount = fi.GetFiles().LongLength;
                result.Properties = new ResultProperties
                                        {
                                            {"CreationTimeUtc", fi.CreationTimeUtc.ToString()},
                                            {"LastAccessTimeUtc", fi.LastAccessTimeUtc.ToString()},
                                            {"LastWriteTimeUtc", fi.LastWriteTimeUtc.ToString()},
                                            {"FileCount", fi.GetFiles().LongLength.ToString()},
                                            {"FolderCount", fi.GetDirectories().LongLength.ToString()},
                                            {"Attributes", fi.Attributes.ToString()}
                                        };
            }

            Messenger.Publish(result);
        }
    }
}