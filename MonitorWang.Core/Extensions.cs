using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MonitorWang.Core.Interfaces;

namespace MonitorWang.Core
{
    public static class Extensions
    {
        public static string TrimEnd(this string s, string remove)
        {
            return !s.EndsWith(remove, StringComparison.InvariantCultureIgnoreCase) 
                ? s 
                : s.Substring(0, s.Length - remove.Length);
        }

        public static IEnumerable<string> Lines(this StreamReader source)
        {
            string line;

            if (source == null)
                throw new ArgumentNullException("source");

            while ((line = source.ReadLine()) != null)
            {
                yield return line;
            }
        }

        /// <summary>
        /// Provides an inline conditional append format method. The supplied text is
        /// only appended if the <paramref name="test"/> is true.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="test"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder AppendIf(this StringBuilder sb, bool test, string format, params object[] args)
        {
            return !test ? sb : sb.AppendFormat(format, args);
        }

        /// <summary>
        /// Provides an inline conditional append format method. The supplied text is
        /// only appended if the <paramref name="test"/> is true.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="test"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder AppendLineIf(this StringBuilder sb, bool test, string format, params object[] args)
        {
            return !test ? sb : sb.AppendLine(string.Format(format, args));
        }

        public static bool InitialiseIfEnabled(this IPlugin plugin)
        {
            if (plugin is ICanBeSwitchedOff)
            {
                if (!((ICanBeSwitchedOff)plugin).Enabled)
                    return false;
            }

            Logger.Debug("Initialising component...'{0}'", plugin.GetType().Name);
            plugin.Initialise();
            return true;
        }
    }
}