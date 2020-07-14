
namespace GapTraderCore.Interfaces
{
    public interface IJournalTrade : ITrade
    {
        ISelectableStrategy Strategy { get; }

        ISelectable Market { get; }
    }
}
