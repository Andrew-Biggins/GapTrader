using GapTraderCore.Interfaces;
using TradeJournalCore.Interfaces;
using TradingSharedCore;
using TradingSharedCore.Interfaces;
using TradingSharedCore.Strategies;
using TradingSharedCore.ViewModels;

namespace TradeJournalCore.ViewModels
{
    public class StrategyDetailsViewModel : TrailingStopBaseViewModel, IStrategyDetails
    {
        public bool HasError
        {
            get => _hasError;
            protected set => SetProperty(ref _hasError, value, nameof(HasError));
        }

        public StrategyDetailsViewModel(StrategyType strategyType)
        {
            StrategyType = strategyType;
        }

        public virtual ISelectableStrategy GetNewStrategy()
        {
            switch (StrategyType)
            {
                case StrategyType.Triangle:
                    return new TriangleStrategy("Triangle", "Triangle", IsStopTrailed, TrailedStopSize);
                case StrategyType.FailedTriangle:
                    return new TriangleStrategy("Failed Triangle", "Failed Triangle", IsStopTrailed, TrailedStopSize);
                default:
                    return new TriangleStrategy("Other", "Other", IsStopTrailed, TrailedStopSize);
            }
        }

        protected readonly StrategyType StrategyType;
        private bool _hasError;
    }

}

