using System.Runtime.Serialization;

namespace MonitorWang.Core.Geckoboard.Entities
{
    [DataContract(Name="root", Namespace = "")]
    public class GeckoComparison : GeckoData
    {
        [DataMember(Name = "item")]
        public GeckoDataItem[] Item { get; set; }

        public GeckoComparison()
        {
            Item = new[]
                       {
                           new GeckoDataItem(),
                           new GeckoDataItem()
                       };
        }

        public GeckoComparison(int decimalPlaces)
            : this()
        {
            Item[0].DecimalPlaces = decimalPlaces;
            Item[1].DecimalPlaces = decimalPlaces;
        }

        public GeckoDataItem Number 
        {
            get { return Item[0]; }
            set { Item[0] = value; }
        }

        public GeckoDataItem Comparison 
        {
            get { return Item[1]; }
            set { Item[1] = value; }
        }
    }
}