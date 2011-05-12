using System;
using System.Runtime.Serialization;

namespace MonitorWang.Core.Geckoboard.Entities
{
    [DataContract(Name="root", Namespace = "")]
    public class GeckoMeter : GeckoData
    {
        private double myItem;

        [DataMember(Name = "item")]
        public double Item
        {
            get { return Math.Round(myItem, DecimalPlaces); }
            set { myItem = value; }
        }

        [DataMember(Name = "min")]
        public GeckoDataItem Min { get; set; }

        [DataMember(Name = "max")]
        public GeckoDataItem Max { get; set; }

        public GeckoMeter()
        {
            Min = new GeckoDataItem();
            Max = new GeckoDataItem();
        }

        public override int DecimalPlaces
        {
            get { return base.DecimalPlaces; }
            set
            {
                base.DecimalPlaces = value;

                if (Min != null)
                    Min.DecimalPlaces = value;
                if (Max != null)
                    Max.DecimalPlaces = value;
            }
        }
    }
}