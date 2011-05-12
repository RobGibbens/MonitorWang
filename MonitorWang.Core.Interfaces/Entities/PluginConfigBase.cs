
using System.Collections.Generic;
using System.Linq;

namespace MonitorWang.Core.Interfaces.Entities
{
    public class PluginConfigBase : ConfigBase
    {
        /// <summary>
        /// a unique friendly id/name for this test
        /// </summary>
        public string FriendlyId { get; set; }

        /// <summary>
        /// This is a simple helper that will determine whether a parameter
        /// value passed from the configuration is in the list of supported
        /// values. This performs a case insensitive comparison
        /// </summary>
        /// <param name="value">The config value to check</param>
        /// <param name="list">A list of valid values</param>
        /// <returns>true if the value is in the list</returns>
        protected bool ParameterValueIsInList(string value, params string[] list)
        {
            return ParameterValueIsInList(value, true, list);
        }
        
        /// <summary>
        /// This is a simple helper that will determine whether a parameter
        /// value passed from the configuration is in the list of supported
        /// values. 
        /// </summary>
        /// <param name="value">The config value to check</param>
        /// <param name="list">A list of valid values</param>
        /// <param name="ignoreCase">pass true to perform a case insensitive comparison</param>
        /// <returns>true if the value is in the list</returns>
        protected bool ParameterValueIsInList(string value, bool ignoreCase, params string[] list)
        {
            List<string> validValues;

            if (ignoreCase)
            {
                validValues = (from item in list select item.ToLowerInvariant()).ToList();
                value = value.ToLowerInvariant();
            }
            else
                validValues = new List<string>(list);    

            return (validValues.Contains(value));
        }
    }
}