using System.Collections.Generic;
using Foundations;
using GapTraderCore.Interfaces;

namespace GapTraderCore.ViewModels
{
    public sealed class MainViewModel : BindableBase
    {
        public MarketDetailsViewModel MarketDetailsViewModel { get; }

        public GapFillStrategyTesterViewModel StrategyTesterViewModel { get; }
        
        public TradeJournalViewModel TradeJournalViewModel { get; }

        public MainViewModel(IRunner runner, List<SavedData> savedData)
        {
            StrategyTesterViewModel = new GapFillStrategyTesterViewModel(_market, runner);
            MarketDetailsViewModel = new MarketDetailsViewModel(savedData, runner, _market);
            TradeJournalViewModel = new TradeJournalViewModel(runner);
        }

        private readonly IMarket _market = new Market();
    }
}
