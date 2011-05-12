using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class URLSCANLogParserCheckConfig : SqlScalarCheckConfig
    {
        public string CheckpointFile { get; set; }
    }

    public class URLSCANLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly URLSCANLogParserCheckConfig myConfig;

        public URLSCANLogParserCheck(URLSCANLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("URLSCAN LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("0E16424C-1BE2-4ae4-A239-9826AD399B8C")
                             };
        }

        /// <summary>
        /// Returns the correct Input Context class for this type of query. It will also sanity check
        /// all relevant config params and throw exceptions where there are violations (required
        /// setting missing, badly formed etc)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if a setting is missing</exception>
        /// <exception cref="FormatException">Thrown if a setting is badly formed</exception>
        protected override object GetInputContext()
        {
            var context = new COMURLScanLogInputContextClass();

            if (!string.IsNullOrEmpty(myConfig.CheckpointFile))
                context.iCheckpoint = myConfig.CheckpointFile;

            return context;
        }
    }
}