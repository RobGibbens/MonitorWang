using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class XMLLogParserCheckConfig : SqlScalarCheckConfig
    {
        public string RootXPath { get; set; }
        public string fMode { get; set; }
        public string TimestampFormat { get; set; }
        public int? NumberOfLeafNodes { get; set; }

        /// <summary>
        /// Compact | XPath 
        /// </summary>
        public string FieldNames { get; set; }
    }

    public class XMLLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly XMLLogParserCheckConfig myConfig;

        public XMLLogParserCheck(XMLLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("XML LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("2A14E4DE-972A-44a1-9754-04457E5AFCDD")
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
            var context = new COMXMLInputContextClass
                              {
                                  fNames = myConfig.FieldNames ?? "Compact"
                              };

            if (myConfig.NumberOfLeafNodes.HasValue)
                context.dtNodes = myConfig.NumberOfLeafNodes.Value;
            if (!string.IsNullOrEmpty(myConfig.TimestampFormat))
                context.iTsFormat = myConfig.TimestampFormat;
            if (!string.IsNullOrEmpty(myConfig.fMode))
                context.fMode = myConfig.fMode;
            if (!string.IsNullOrEmpty(myConfig.RootXPath))
                context.rootXPath = myConfig.RootXPath;

            return context;
        }
    }
}