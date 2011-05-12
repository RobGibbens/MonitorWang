using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;

namespace MonitorWang.Agent
{
    /// <summary>
    /// Simple class to model the custom command line args we use. This
    /// encapsulates the runtime argument list and contain helpers to filter
    /// and select custom arg values
    /// </summary>
    public class CmdLine
    {
        public class SwitchNames
        {
            public const string Profile = "Profile";
            public const string Install = "Install";
            public const string Uninstall = "Uninstall";
            public const string Username = "Username";
            public const string Password = "Password";
        }

        protected static CmdLineArgs myArgs;
        protected static CmdLineArgs myExpandedArgs;
        protected static CmdLineSwitches mySupportedSwitches;
        
        static CmdLine()
        {
            // setup our supported list of cmdline switches
            mySupportedSwitches = CmdLineSwitches.Init(
                                       CustomSwitch.Build(SwitchNames.Profile),
                                       CustomSwitch.Build(SwitchNames.Username),
                                       CustomSwitch.Build(SwitchNames.Password),
                                       CustomSwitch.Build(SwitchNames.Install, a => a.Add("install")),
                                       CustomSwitch.Build(SwitchNames.Uninstall, a => a.Add("uninstall")));
        }

        public static void Init(IEnumerable<string> args)
        {
            myArgs = CmdLineArgs.Init(args);
            myExpandedArgs = CmdLineArgs.Init();
            // expand custom args into topshelf args
            myArgs.IfSupplied(mySupportedSwitches, s => s.Expand(myExpandedArgs));
        }

        /// <summary>
        /// This will return the value of the switch - it assumes a string
        /// value so will use a : as the switch delimiter eg: /switch:value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Value(string name, out string value)
        {
            var switchName = "/" + name.ToLower();
            value = string.Empty;

            var values = from arg in myArgs
                         where arg.StartsWith(switchName)
                         select arg;

            if (values.Count() == 0)
                return false;

            value = values.First().Replace(switchName + ":", string.Empty);
            return true;
        }

        /// <summary>
        /// This will return a list of all custom cmdline switches
        /// </summary>
        public static IEnumerable<string> All
        {
            get { return myArgs.Concat(myExpandedArgs); }
        }

        /// <summary>
        /// This will return the set of expanded arguments
        /// </summary>
        public static IEnumerable<string> Expanded
        {
            get { return myExpandedArgs; }
        }

        /// <summary>
        /// This will return a list of all custom cmdline switches
        /// </summary>
        public static CmdLineSwitches Switches
        {
            get { return mySupportedSwitches; }
        }
    }

    public class CmdLineArgs : List<string>
    {
        private CmdLineArgs(IEnumerable<string> collection)
            : base(collection)
        {
        }

        public static CmdLineArgs Init(IEnumerable<string> args)
        {
            return new CmdLineArgs(args);
        }

        public static CmdLineArgs Init(params string[] args)
        {
            return new CmdLineArgs(args);
        }

        public void IfSupplied(CmdLineSwitches switches, Action<CustomSwitch> switchAction)
        {
            switches.Intersect(this).ForEach(switchAction);
        }
    }

    public class CmdLineSwitches : List<CustomSwitch>
    {
        private CmdLineSwitches(IEnumerable<CustomSwitch> collection) : base(collection)
        {
        }

        public static CmdLineSwitches Init(IEnumerable<CustomSwitch> switches)
        {
            return new CmdLineSwitches(switches);
        }

        public static CmdLineSwitches Init(params CustomSwitch[] switches)
        {
            return new CmdLineSwitches(switches);
        }

        public IEnumerable<string> Cast()
        {
            return (from item in this
                   select item.Switch).ToList();
        }

        public IEnumerable<CustomSwitch> Intersect(IEnumerable<string> args)
        {
            var comparer = CustomSwitch.ArgComparer;

            return from item in this
                   where args.Contains(item.Switch, comparer)
                   select item;
        }
    }

    public class CustomSwitch : IEqualityComparer<string>
    {
        public string Name { get; set; }
        public string Switch { get { return "/" + Name; } }

        protected Action<List<string>> myExpander { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static IEqualityComparer<string> ArgComparer 
        { 
            get { return new CustomSwitch(); }
        }

        /// <summary>
        /// Helper to contruct a custom switch
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CustomSwitch Build(string name)
        {
            return new CustomSwitch
            {
                Name = name
            };
        }

        /// <summary>
        /// Helper to contruct a custom switch
        /// </summary>
        /// <param name="name"></param>
        /// <param name="expand"></param>
        /// <returns></returns>
        public static CustomSwitch Build(string name, Action<List<string>> expand)
        {
            return new CustomSwitch
            {
                Name = name,
                myExpander = expand
            };
        }

        /// <summary>
        /// This will execute the custom expansion logic (if supplied via
        /// <see cref="Build(string,System.Action{System.Collections.Generic.List{string}})"/>)
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CustomSwitch Expand(List<string> args)
        {
            if (myExpander == null)
                return this;

            myExpander.Invoke(args);
            return this;
        }

        public bool Equals(string arg, string switchName)
        {
            return arg.ToLowerInvariant().StartsWith(switchName.ToLowerInvariant());
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}