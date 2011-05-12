using System;
using System.IO;
using MonitorWang.Core.Interfaces;
using MonitorWang.Core.Interfaces.Entities;

namespace MonitorWang.Core.Checks
{
    public class FileInfoCheckConfig : PluginConfigBase
    {
        public string FileLocation { get; set; }
    }

    public class FileInfoCheck : IHealthCheckPlugin
    {
        protected readonly FileInfoCheckConfig myConfig;
        protected readonly SmartLocation myFileLocation;
        protected PluginDescriptor  myIdentity;

        /// <summary>
        /// default ctor
        /// </summary>
        public FileInfoCheck(FileInfoCheckConfig config)
        {
            myConfig = config;
            myFileLocation = new SmartLocation(config.FileLocation);
            myIdentity = new PluginDescriptor
            {
                Description = string.Format("Reports information about file {0}", myFileLocation.Location),
                TypeId = new Guid("E0DDCF22-25B6-4d05-B2C4-D1EBFBEBB681"),
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

        public void Execute()
        {
            var fi = new FileInfo(myFileLocation.Location);
            var result = new HealthCheckData
                             {
                                 Identity = Identity,
                                 Info = string.Format("Information about file '{0}'...", myFileLocation.Location),
                                 Result = fi.Exists
                             };

            if (result.Result.GetValueOrDefault(false))
            {
                result.ResultCount = fi.Length;
                result.Properties = new ResultProperties
                                        {
                                            {"CreationTimeUtc", fi.CreationTimeUtc.ToString()},
                                            {"LastAccessTimeUtc", fi.LastAccessTimeUtc.ToString()},
                                            {"LastWriteTimeUtc", fi.LastWriteTimeUtc.ToString()},
                                            {"Length", fi.Length.ToString()},
                                            {"Attributes", fi.Attributes.ToString()}
                                        };
            }

            Messenger.Publish(result);
        }
    }
}