using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class IISLogParserCheckConfig : SqlScalarCheckConfig
    {
        /// <summary>
        /// 0 is the system codepage; -2 specifies that the codepage is automatically 
        /// determined by inspecting the filename and/or the site's "LogInUTF8" property. 
        /// </summary>
        public int? Codepage { get; set; }

        /// <summary>
        /// 0 disables subdirectory recursion
        /// -1 enables unlimited recursion
        /// other wise is the level to recurse
        /// </summary>
        public int? Recurse { get; set; }

        /// <summary>
        /// date/time (in "yyyy-MM-dd hh:mm:ss" format)
        /// </summary>
        public string MinDateMod { get; set; }

        /// <summary>
        /// default is DEF
        /// </summary>
        public string Locale { get; set; }

        public string CheckpointFile { get; set; }
    }

    public class IISLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly IISLogParserCheckConfig myConfig;

        public IISLogParserCheck(IISLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;

            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("IIS LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("FA8E0B46-43E5-433b-B417-21216531F92F")
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
            var context = new COMIISIISInputContextClass
                              {
                                  iCodepage = myConfig.Codepage.GetValueOrDefault(-2),
                                  recurse = myConfig.Recurse.GetValueOrDefault(0),
                                  locale = myConfig.Locale ?? "DEF"
                              };

            if (!string.IsNullOrEmpty(myConfig.MinDateMod))
                context.minDateMod = myConfig.MinDateMod;
            if (!string.IsNullOrEmpty(myConfig.CheckpointFile))
                context.iCheckpoint = myConfig.CheckpointFile;

            return context;
        }
    }
}