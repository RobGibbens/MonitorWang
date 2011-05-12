using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class TextLineLogParserCheckConfig : SqlScalarCheckConfig
    {
        public int? CodePage { get; set; }
        public int? Recurse { get; set; }
        public bool? SplitLongLines { get; set; }
        public string CheckpointFile { get; set; }
    }

    public class TextLineLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly TextLineLogParserCheckConfig myConfig;

        public TextLineLogParserCheck(TextLineLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("TextLine LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("22C41BC0-DDCA-4d6b-830A-F47959F1A516")
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
            var context = new COMTextLineInputContextClass
                              {
                                  codepage = myConfig.CodePage ?? 0,
                                  recurse = myConfig.Recurse ?? 0,
                                  splitLongLines = myConfig.SplitLongLines ?? false
                              };

            if (!string.IsNullOrEmpty(myConfig.CheckpointFile))
                context.iCheckpoint = myConfig.CheckpointFile;

            return context;
        }
    }
}