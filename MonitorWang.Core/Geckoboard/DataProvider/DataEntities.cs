using System;

namespace MonitorWang.Core.Geckoboard.DataProvider
{
    public enum DataOperationType
    {
        Count = 0,
        Sum,
        Average,
        Max,
        Min
    }

    public enum OutcomeType
    {
        Any = 0,
        Failure,
        Success
    }

    public struct PieChartData
    {
        public string Label { get; set; }
        public long Count { get; set; }
    }

    public class LineChartArgs
    {
        public string Agent { get; set;}
        public string Site { get; set; }
        public string Check { get; set; }
        public string Tag { get; set; }
        public long Limit { get; set; }
        public int MaxItems { get; set; }
        public int Multiplier { get; set; }
        public string Bucket { get; set; }
        public int DecimalPlaces { get; set; }
        public int Sample { get; set; }
        public string Unit { get; set; }
        public OutcomeType Outcome { get; set; }
        public DateTime EndDate { get; set; }
        public DataOperationType DataOperation { get; set; }
        public int ScaleUp { get; set; }
        public int ScaleDown { get; set; }
        public int ScaleYDecimalPlaces { get; set; }
        public bool ScaleY { get; set; }

        public LineChartArgs()
        {
            Sample = 1;
            Multiplier = 1;
            ScaleUp = 1;
            ScaleDown = 1;
            ScaleY = true;

            var now = DateTime.UtcNow;
            EndDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0);
        }
    }

    public class PieChartArgs
    {
        public string Site { get; set; }
        public string Agent { get; set; }
        public string Check { get; set; }
        public OutcomeType Outcome { get; set; }
        public DataOperationType DataOperation { get; set; }       
        public long Limit { get; set; }
        public int DecimalPlaces { get; set; }
    }

    public struct LineChartData
    {
        public double Value { get; set; }
        public DateTime When { get; set; }
    }

    public class GeckometerArgs
    {
        public string Site { get; set; }
        public string Check { get; set; }
        public string Tag { get; set; }
        public int DecimalPlaces { get; set; }
    }

    public struct GeckometerData
    {
        public double Min { get; set; }
        public double Avg { get; set; }
        public double Max { get; set; }
    }

    public class ComparisonArgs
    {
        public string Site { get; set; }
        public string Check { get; set; }
        public string Tag { get; set; }
        public int DecimalPlaces { get; set; }
    }

    public struct ComparisonData
    {
        public double Number { get; set; }
        public double Comparison { get; set; }
    }
}