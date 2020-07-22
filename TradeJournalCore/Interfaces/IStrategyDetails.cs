using TradingSharedCore.Interfaces;

namespace TradeJournalCore.Interfaces
{
    public interface IStrategyDetails
    {
        bool HasError { get; }

        ISelectableStrategy GetNewStrategy();
    }
}