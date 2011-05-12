using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class CSVLogParserCheckConfig : SqlScalarCheckConfig
    {
        public bool? HeaderRow { get; set; }
        public string HeaderFile { get; set; }
        public bool? FixedFields { get; set; }
        public int? Fields { get; set; }
        public int? Lines { get; set; }

        /// <summary>
        /// Valid values are "Auto" or "Ignore"
        /// </summary>
        public string DoubleQuotes { get; set; }

        public int? SkipLines { get; set; }
        public string Comment { get; set; }
        public int? CodePage { get; set; }
        public string TimestampFormat { get; set; }
        public string CheckpointFile { get; set; }
    }

    public class CSVLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly CSVLogParserCheckConfig myConfig;

        public CSVLogParserCheck(CSVLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("CSV LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("591DFDD0-FC26-44de-AF84-42C2BB20421D")
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
            var context = new COMCSVInputContextClass
                              {
                                  headerRow = myConfig.HeaderRow ?? true,
                                  fixedFields = myConfig.FixedFields ?? true,
                                  nFields = myConfig.Fields ?? -1,
                                  dtLines = myConfig.Lines ?? 10,
                                  iDQuotes = myConfig.DoubleQuotes ?? "Auto",
                                  nSkipLines = myConfig.SkipLines ?? 0,
                                  codepage = myConfig.CodePage ?? 0
                              };

            if (!string.IsNullOrEmpty(myConfig.TimestampFormat))
                context.iTsFormat = myConfig.TimestampFormat;
            if (!string.IsNullOrEmpty(myConfig.HeaderFile))
                context.headerFile = myConfig.HeaderFile;
            if (!string.IsNullOrEmpty(myConfig.Comment))
                context.comment = myConfig.Comment;
            if (!string.IsNullOrEmpty(myConfig.CheckpointFile))
                context.iCheckpoint = myConfig.CheckpointFile;

            return context;
        }
    }
}