using CombinedCore.Interfaces;
using GapTraderCore.ViewModels;
using TradeJournalCore.ViewModels;
using TradingSharedCore.Interfaces;

namespace CombinedCore.ViewModels
{
    public sealed class CombinedMainViewModel : GapTraderMainViewModel
    {
        public TradeJournalViewModel TradeJournalViewModel { get; }

        public CombinedMainViewModel(ICombinedRunner runner) : base(runner)
        { 
            TradeJournalViewModel = new TradeJournalViewModel(runner);
        }
    }
}
