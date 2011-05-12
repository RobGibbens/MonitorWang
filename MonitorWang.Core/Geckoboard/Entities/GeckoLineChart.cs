using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MonitorWang.Core.Geckoboard.Entities
{
    [DataContract(Name="root", Namespace = "")]
    public class GeckoLineChart : GeckoData
    {
        [DataContract(Namespace = "")]
        public class Setting : GeckoData
        {
            [DataMember(Name = "axisx")]
            public List<string> X { get; set; }
            [DataMember(Name = "axisy")]
            public List<string> Y { get; set; }
            [DataMember(Name = "colour")]
            public string Colour { get; set; }

            public Setting()
            {
                X = new List<string>();
                Y = new List<string>();
            }
        }

        [DataMember(Name = "item")]
        public List<string> Item { get; set; }

        [DataMember(Name = "settings")]
        public Setting Settings { get; set; }

        public GeckoLineChart()
        {
            Item = new List<string>();
            Settings = new Setting();
        }
    }
}