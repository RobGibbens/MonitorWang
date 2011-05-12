using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MonitorWang.Core.Geckoboard.Entities
{
    [DataContract(Name="root", Namespace = "")]
    public class GeckoPieChart
    {
        [DataContract(Namespace = "")]
        public class Piece : GeckoData
        {
            private double myValue;

            [DataMember(Name = "value")]
            public double Value
            {
                get { return Math.Round(myValue, DecimalPlaces); }
                set { myValue = value; }
            }
            [DataMember(Name = "label")]
            public string Label { get; set; }
            [DataMember(Name = "colour")]
            public string Colour { get; set; }
        }

        [DataMember(Name = "item")]
        public List<Piece> Item { get; set; }

        public GeckoPieChart()
        {
            Item = new List<Piece>();
        }
    }
}