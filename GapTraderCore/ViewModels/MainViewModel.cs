using Foundations;
using GapTraderCore.Interfaces;

namespace GapTraderCore.ViewModels
{
    public sealed class MainViewModel : BindableBase
    {
        public MarketDetailsViewModel MarketDetailsViewModel { get; }

        public GapFillStrategyTesterViewModel StrategyTesterViewModel { get; private set; }
        
        public TradeJournalViewModel TradeJournalViewModel { get; }

        public MainViewModel(IRunner runner)
        {
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market, runner);
            MarketDetailsViewModel = new MarketDetailsViewModel(runner, _market);
            TradeJournalViewModel = new TradeJournalViewModel(runner);

            MarketDetailsViewModel.MarketDataChanged += (s, e) =>
            {
                StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market, runner);
                RaisePropertyChanged(nameof(StrategyTesterViewModel));

                StrategyTesterViewModel.StrategyFinderViewModel.DataInUseToggle +=
                    (x, y) => MarketDetailsViewModel.ToggleDataInUse();
            };
        }

        private readonly IMarket _market = new Market();
    }
}
