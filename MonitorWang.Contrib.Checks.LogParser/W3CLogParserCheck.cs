using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class W3CLogParserCheckConfig : SqlScalarCheckConfig
    {
        public int? CodePage { get; set; }
        public int? Lines { get; set; }
        public bool? DoubleQuoted { get; set; }
        public string Separator { get; set; }
    }

    public class W3CLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly W3CLogParserCheckConfig myConfig;

        public W3CLogParserCheck(W3CLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("W3C LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("7A633E92-4211-4efe-B557-9A8258DD016C")
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
            var context = new COMW3CInputContextClass
                              {
                                  dtLines = myConfig.Lines ?? 10,
                                  codepage = myConfig.CodePage ?? 0,
                                  doubleQuotedStrings = myConfig.DoubleQuoted ?? false
                              };

            if (!string.IsNullOrEmpty(myConfig.Separator))
                context.separator = myConfig.Separator;

            return context;
        }
    }
}