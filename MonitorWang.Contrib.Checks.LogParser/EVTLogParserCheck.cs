using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class EVTLogParserCheckConfig : SqlScalarCheckConfig
    {
        public bool? FullText { get; set; }
        public bool? ResolveSIDs { get; set; }
        public bool? FormatMsg { get; set; }
        public bool? FullEventCode { get; set; }
        public string BinaryFormat { get; set; }
        public string CheckpointFile { get; set; }
        public string StringsSep { get; set; }
        public string MsgErrorMode { get; set; }
        public string Direction { get; set; }
    }

    public class EVTLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly EVTLogParserCheckConfig myConfig;

        public EVTLogParserCheck(EVTLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("EventLog LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("451EE20A-2938-47cc-B972-050CABB1DBBE")
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
            var context = new COMEventLogInputContextClassClass
                              {
                                  fullText = myConfig.FullText.GetValueOrDefault(true),
                                  resolveSIDs = myConfig.ResolveSIDs.GetValueOrDefault(false),
                                  formatMsg = myConfig.FormatMsg.GetValueOrDefault(true),
                                  fullEventCode = myConfig.FullEventCode.GetValueOrDefault(false),
                                  msgErrorMode = myConfig.MsgErrorMode ?? "MSG",
                                  direction = myConfig.Direction ?? "FW",
                                  stringsSep = myConfig.StringsSep ?? "|",
                                  binaryFormat = myConfig.BinaryFormat ?? "HEX"
                              };

            if (!string.IsNullOrEmpty(myConfig.CheckpointFile))
                context.iCheckpoint = myConfig.CheckpointFile;

            return context;
        }
    }
}