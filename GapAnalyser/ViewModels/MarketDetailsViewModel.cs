using Foundations;
using GapAnalyser.Interfaces;

namespace GapAnalyser.ViewModels
{
    public sealed class MarketDetailsViewModel : BindableBase
    {
        public IMarket Market
        {
            get => _market;
            set => SetProperty(ref _market, value);
        }

        public string DataStartDate { get; } = string.Empty;
        public string DataEndDate { get; } = string.Empty;
        public string DataHigh { get; } = string.Empty;
        public string HighDate { get; } = string.Empty;
        public string DataLow { get; } = string.Empty;
        public string LowDate { get; } = string.Empty;
        public string AverageGapSize { get; } = string.Empty;

        public MarketDetailsViewModel()
        {

        }

        public MarketDetailsViewModel(IMarket market)
        {
            Market = market;

            DataStartDate = $"{market.DataDetails.StartDate:d/M/yy}";
            DataEndDate = $"{market.DataDetails.EndDate:d/M/yy}";
            DataHigh = $"{market.DataDetails.High:N1}";
            HighDate = $"{market.DataDetails.HighDate:d/M/yy}";
            DataLow = $"{market.DataDetails.Low:N1}";
            LowDate = $"{market.DataDetails.LowDate:d/M/yy}";
            AverageGapSize = $"{market.DataDetails.AverageGapSize:N1}";
        }

        private IMarket _market = new Market();
    }
}
