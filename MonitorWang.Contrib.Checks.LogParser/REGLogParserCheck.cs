using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class REGLogParserCheckConfig : SqlScalarCheckConfig
    {
        public int? Recurse { get; set; }

        /// <summary>
        /// default = "|"
        /// </summary>
        public string MultiSZSep { get; set; }

        /// <summary>
        /// Valid values are "ASC", "HEX", "PRINT"
        /// </summary>
        public string BinaryFormat { get; set; }
    }

    public class REGLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly REGLogParserCheckConfig myConfig;

        public REGLogParserCheck(REGLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("Registry LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("6725AA64-376F-415c-8F36-E2DE1889EBC0")
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
            var context = new COMRegistryInputContextClass
                              {
                                  recurse = myConfig.Recurse ?? -1,
                                  multiSZSep = myConfig.MultiSZSep ?? "|",
                                  binaryFormat = myConfig.BinaryFormat ?? "ASC"
                              };

            return context;
        }
    }
}