using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class TSVLogParserCheckConfig : SqlScalarCheckConfig
    {
        /// <summary>
        /// Valid values are a single character | spaces | space | tab (backslash t)
        /// </summary>
        public string Separator { get; set; }

        public int? NumberOfSeparators { get; set; }
        public bool? FixedSeparator { get; set; }
        public bool? HeaderRow { get; set; }
        public string HeaderFile { get; set; }
        public int? Fields { get; set; }
        public int? Lines { get; set; }
        public int? SkipLines { get; set; }
        public string LineFilter { get; set; }
        public int? CodePage { get; set; }
        public string TimestampFormat { get; set; }
        public string CheckpointFile { get; set; }
    }

    public class TSVLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly TSVLogParserCheckConfig myConfig;

        public TSVLogParserCheck(TSVLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("TSV LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("A82F0463-387D-4e80-AD62-A5FDBDD78DD1")
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
            var context = new COMTSVInputContextClass
                              {
                                  nSeparators = myConfig.NumberOfSeparators ?? 1,
                                  fixedSep = myConfig.FixedSeparator ?? false,
                                  headerRow = myConfig.HeaderRow ?? true,
                                  dtLines = myConfig.Lines ?? 100,
                                  nSkipLines = myConfig.SkipLines ?? 0,
                                  codepage = myConfig.CodePage ?? 0
                              };

            if (myConfig.Fields.HasValue)
                context.nFields = myConfig.Fields.Value;
            if (!string.IsNullOrEmpty(myConfig.Separator))
                context.iSeparator = myConfig.Separator;
            if (!string.IsNullOrEmpty(myConfig.TimestampFormat))
                context.iTsFormat = myConfig.TimestampFormat;
            if (!string.IsNullOrEmpty(myConfig.HeaderFile))
                context.headerFile = myConfig.HeaderFile;
            if (!string.IsNullOrEmpty(myConfig.CheckpointFile))
                context.iCheckpoint = myConfig.CheckpointFile;
            if (!string.IsNullOrEmpty(myConfig.LineFilter))
                context.lineFilter = myConfig.LineFilter;

            return context;
        }
    }
}