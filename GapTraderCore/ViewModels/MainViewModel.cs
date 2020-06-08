using System.Collections.Generic;
using System.ComponentModel;
using Foundations;
using GapTraderCore.Interfaces;

namespace GapTraderCore.ViewModels
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

        public MainViewModel(IRunner runner, List<SavedData> savedData)
        {
            _runner = runner;
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market, runner);
            MarketDetailsViewModel = new MarketDetailsViewModel(savedData, _runner, _market);
            _market.PropertyChanged += OnMarketDataChanged;
        }

        private void OnMarketDataChanged(object sender, PropertyChangedEventArgs e)
        {
            GapFillStatsViewModel = new GapFillStatsViewModel(_market);
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market, _runner);
        }

        private readonly IRunner _runner;
        private readonly IMarket _market = new Market();

        private GapFillStatsViewModel _gapFillStatsViewModel = new GapFillStatsViewModel();
        private MarketDetailsViewModel _marketDetailsViewModel;
        private GapFillStrategyTesterViewModel _strategyTesterViewModel;
    }
}
