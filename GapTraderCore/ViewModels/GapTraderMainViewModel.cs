using GapTraderCore.Interfaces;
using TradingSharedCore;

namespace GapTraderCore.ViewModels
{
    public class GapTraderMainViewModel : BindableBase
    {
        public MarketDetailsViewModel MarketDetailsViewModel { get; }

        public GapFillStrategyTesterViewModel StrategyTesterViewModel { get; private set; }
        

        public GapTraderMainViewModel(IGapTraderRunner runner)
        {
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market, runner);
            MarketDetailsViewModel = new MarketDetailsViewModel(runner, _market);

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
