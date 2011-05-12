using System;
using MonitorWang.Core.Checks;
using MonitorWang.Core.Interfaces.Entities;
using MSUtil;

namespace MonitorWang.Contrib.Checks.LogParser
{
    public class IISW3CLogParserCheckConfig : SqlScalarCheckConfig
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
        /// 
        /// </summary>
        public bool? DoubleQuotes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? DirTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? ConsolidateLogs { get; set; }

        public string CheckpointFile { get; set; }
    }

    public class IISW3CLogParserCheck : ScalarLogParserCheckBase
    {
        protected readonly IISW3CLogParserCheckConfig myConfig;

        public IISW3CLogParserCheck(IISW3CLogParserCheckConfig config)
            : base(config)
        {
            myConfig = config;

            myIdentity = new PluginDescriptor
                             {
                                 Description = string.Format("IISW3C LogParser Check"),
                                 Name = config.FriendlyId,
                                 TypeId = new Guid("2C98A60B-88B3-4333-965C-08A41789B7A3")
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
            var context = new COMIISW3CInputContextClass
                              {
                                  iCodepage = myConfig.Codepage.GetValueOrDefault(-2),
                                  recurse = myConfig.Recurse.GetValueOrDefault(0),
                                  consolidateLogs = myConfig.ConsolidateLogs.GetValueOrDefault(false),
                                  dirTime = myConfig.DirTime.GetValueOrDefault(false),
                                  dQuotes = myConfig.DoubleQuotes.GetValueOrDefault(false)
                              };

            if (!string.IsNullOrEmpty(myConfig.MinDateMod))
                context.minDateMod = myConfig.MinDateMod;
            if (!string.IsNullOrEmpty(myConfig.CheckpointFile))
                context.iCheckpoint = myConfig.CheckpointFile;

            return context;
        }
    }
}