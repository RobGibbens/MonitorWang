using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class FSLogParserCheckConfig : SqlScalarCheckConfig
    {
        public int? Recurse { get; set; }
        public bool? PreserveLastAccTime { get; set; }
        public bool? UseLocalTime { get; set; }
    }

    public class FSLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly FSLogParserCheckConfig myConfig;

        public FSLogParserCheck(FSLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;
            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("FileSystem LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("64717308-4E6D-4b54-896C-EA0C40229287")
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
            var context = new COMFileSystemInputContextClass
                              {
                                  recurse = myConfig.Recurse ?? -1,
                                  preserveLastAccTime = myConfig.PreserveLastAccTime ?? false,
                                  useLocalTime = myConfig.UseLocalTime ?? true
                              };

            return context;
        }
    }
}