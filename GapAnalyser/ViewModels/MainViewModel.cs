using System.Collections.Generic;
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

        //public MarketDetailsViewModel MarketDetailsViewModel
        //{
        //    get => _marketDetailsViewModel; 
        //    set => SetProperty(ref _marketDetailsViewModel, value);
        //}

        public GapFillStrategyTesterViewModel StrategyTesterViewModel
        {
            get => _strategyTesterViewModel;
            set => SetProperty(ref _strategyTesterViewModel, value);
        }

        public DataUploaderViewModel DataUploaderViewModel { get; }

        public MainViewModel(IRunner runner, List<SavedData> savedData)
        {
            _runner = runner;
            _savedData = savedData;
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market, runner);
            DataUploaderViewModel = new DataUploaderViewModel(_market, runner, _savedData);
        //    MarketDetailsViewModel = new MarketDetailsViewModel(_savedData, _runner);
            _market.PropertyChanged += OnMarketDataChanged;
        }

        private void OnMarketDataChanged(object sender, PropertyChangedEventArgs e)
        {
            GapFillStatsViewModel = new GapFillStatsViewModel(_market);
         //   MarketDetailsViewModel = new MarketDetailsViewModel(_savedData, _market, _runner);
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market, _runner);
        }

        private readonly IRunner _runner;
        private readonly IMarket _market = new Market();
        private readonly List<SavedData> _savedData;

        private GapFillStatsViewModel _gapFillStatsViewModel = new GapFillStatsViewModel();
      //  private MarketDetailsViewModel _marketDetailsViewModel;
        private GapFillStrategyTesterViewModel _strategyTesterViewModel;
    }
}
