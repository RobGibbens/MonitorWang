using System;
using System.Runtime.Serialization;

namespace MonitorWang.Core.Geckoboard.Entities
{
    [DataContract(Namespace = "")]
    public class GeckoDataItem : GeckoData
    {        
        private double myValue;

        [DataMember(Name = "value")]
        public double Value
        {
            get { return Math.Round(myValue, DecimalPlaces); }
            set { myValue = value; }
        }
        [DataMember(Name = "text")]
        public string Text { get; set; }
    }
}