using System;
using System.IO;
using System.ServiceModel;
using System.Web;
using System.Web.Hosting;

namespace MonitorWang.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class SmartLocation
    {
        protected string myRawLocation;

        /// <summary>
        /// Default ctor
        /// </summary>
        public SmartLocation()
        {
        }

        /// <summary>
        /// ctor overload - allows initial location to be set
        /// </summary>
        /// <param name="rawLocation"></param>
        public SmartLocation(string rawLocation)
            : this()
        {
            myRawLocation = Environment.ExpandEnvironmentVariables(rawLocation);
        }

        /// <summary>
        /// The unresolved location 
        /// </summary>
        public string RawLocation
        {
            get { return myRawLocation; }
            set { myRawLocation = value; }
        }

        /// <summary>
        /// The resolved location. If the <see cref="RawLocation"/> is no rooted
        /// and this is a web application then the fully qualified path will be 
        /// resolved relative to the virtual application folder otherwise resolved
        /// to the executing assembly path.
        /// </summary>
        public string Location
        {
            get
            {
                return (Path.IsPathRooted(RawLocation))
                           ? RawLocation
                           : Path.Combine(GetBinFolder(), string.IsNullOrEmpty(RawLocation) ? "." : RawLocation);
            }
        }

        /// <summary>
        /// overrides the default ToString() and returns the <see cref="Location"/> 
        /// property value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Location;
        }

        /// <summary>
        /// Static inline version of the <see cref="Location"/> property
        /// </summary>
        /// <param name="rawLocation">The "raw" location</param>
        /// <returns>The resolved location consistent for the application context/environment</returns>
        public static string GetLocation(string rawLocation)
        {
            var sl = new SmartLocation(rawLocation);
            return sl.Location;
        }

        /// <summary>
        /// Returns the bin folder for the current application type
        /// </summary>
        /// <returns></returns>
        public static string GetBinFolder()
        {
            string location;

            if (HttpContext.Current != null)
            {
                // web (IIS/WCF ASP compatibility mode)context
                location = HttpRuntime.BinDirectory;
            }
            else if (OperationContext.Current != null)
            {
                // pure wcf context
                location = HostingEnvironment.ApplicationPhysicalPath;
            }
            else
            {
                // no special hosting context (console/winform etc)
                location = AppDomain.CurrentDomain.BaseDirectory;
            }

            return location;
        }
    }
}