using GapTraderCore.Interfaces;

namespace GapTraderCore
{
    public class MarketStats : ILoadable
    {
        public string DataStartDate { get; } = string.Empty;
        public string DataEndDate { get; } = string.Empty;
        public string DataHigh { get; } = string.Empty;
        public string HighDate { get; } = string.Empty;
        public string DataLow { get;  } = string.Empty;
        public string LowDate { get; } = string.Empty;
        public string AverageGapSize { get; } = string.Empty;

        public MarketStats()
        {
        }

        public MarketStats(DataDetails stats)
        {
            DataStartDate = $"{stats.StartDate:d/M/yy}";
            DataEndDate = $"{stats.EndDate:d/M/yy}";
            DataHigh = $"{stats.High:N1}";
            HighDate = $"{stats.HighDate:d/M/yy}";
            DataLow = $"{stats.Low:N1}";
            LowDate = $"{stats.LowDate:d/M/yy}";
            AverageGapSize = $"{stats.AverageGapSize:N1}";
        }
    }
}