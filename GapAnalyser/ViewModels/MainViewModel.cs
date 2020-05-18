using Foundations;
using GapAnalyser.Interfaces;
using System.ComponentModel;

namespace GapAnalyser.ViewModels
{
    public sealed class MainViewModel : BindableBase
    {
        public GapFillStatsViewModel GapFillStatsViewModel
        {
            get => _gapFillStatsViewModel;
            set => SetProperty(ref _gapFillStatsViewModel, value);
        }

        public MarketDetailsViewModel MarketDetailsViewModel
        {
            get => _marketDetailsViewModel; 
            set => SetProperty(ref _marketDetailsViewModel, value);
        }

        public GapFillStrategyTesterViewModel StrategyTesterViewModel
        {
            get => _strategyTesterViewModel;
            set => SetProperty(ref _strategyTesterViewModel, value);
        }

        public DataUploaderViewModel DataUploaderViewModel { get; }

        public MainViewModel(IRunner runner)
        {
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market);
            DataUploaderViewModel = new DataUploaderViewModel(_market, runner);
            _market.PropertyChanged += OnMarketDataChanged;
        }

        private void OnMarketDataChanged(object sender, PropertyChangedEventArgs e)
        {
            GapFillStatsViewModel = new GapFillStatsViewModel(_market);
            MarketDetailsViewModel = new MarketDetailsViewModel(_market);
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market);
        }

        private readonly IMarket _market = new Market();
        private GapFillStatsViewModel _gapFillStatsViewModel = new GapFillStatsViewModel();
        private MarketDetailsViewModel _marketDetailsViewModel = new MarketDetailsViewModel();
        private GapFillStrategyTesterViewModel _strategyTesterViewModel = new GapFillStrategyTesterViewModel();
    }
}
