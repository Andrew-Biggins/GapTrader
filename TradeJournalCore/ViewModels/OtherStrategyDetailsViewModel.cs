using TradingSharedCore;
using TradingSharedCore.Interfaces;
using TradingSharedCore.Strategies;

namespace TradeJournalCore.ViewModels
{
    public sealed class OtherStrategyDetailsViewModel : StrategyDetailsViewModel
    {
        public string Name { get; set; } = string.Empty;

        public OtherStrategyDetailsViewModel(StrategyType strategyType) : base(strategyType)
        {
        }

        public override ISelectableStrategy GetNewStrategy()
        {
            // Placeholder until more are added
            return new TriangleStrategy(Name, Name, IsStopTrailed, TrailedStopSize);
        }
    }
}
