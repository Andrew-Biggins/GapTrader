using GapTraderCore.Interfaces;

namespace TradingSharedCore.Interfaces
{
    public interface IJournalTrade : ITrade
    {
        ISelectableStrategy Strategy { get; }

        ISelectable Market { get; }
    }
}
