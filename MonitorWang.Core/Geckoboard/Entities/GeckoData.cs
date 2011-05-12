using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MonitorWang.Core.Geckoboard.Entities
{
    [DataContract(Namespace = "")]
    public class GeckoData
    {
        public virtual int DecimalPlaces { get; set; }
    }

    [DataContract(Namespace = "")]
    public class GeckoValues : List<double>
    {
        public double Mid()
        {
            return (this.Min() + ((this.Max() - this.Min())/2));
        }

        public double Mid(int dp)
        {
            return Math.Round(Mid(), dp);
        }
    }
}