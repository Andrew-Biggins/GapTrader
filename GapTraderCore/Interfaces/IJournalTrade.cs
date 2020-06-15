using Foundations.Optional;

namespace GapTraderCore.Interfaces
{
    public interface IJournalTrade : ITrade
    {
        IStrategy Strategy { get; }
        IMarket Market { get; }
        double RiskRewardRatio { get; }
        Optional<double> ResultInR { get; }
    }
}
