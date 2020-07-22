namespace TradingSharedCore.Interfaces
{
    public interface IGapFillStrategy : IStrategy
    {
        double MinimumGapSize { get; }
    }
}