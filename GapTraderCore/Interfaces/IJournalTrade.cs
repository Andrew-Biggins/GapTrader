using Foundations.Optional;

namespace GapTraderCore.Interfaces
{
    public interface IJournalTrade : ITrade
    {
        ISelectableStrategy Strategy { get; }

        ISelectable Market { get; }

        double RiskRewardRatio { get; }

        Optional<double> ResultInR { get; }
    }
}
