namespace GapTraderCore.Interfaces
{
    public interface IGapFillStrategy : IStrategy
    {
        double MinimumGapSize { get; }
    }
}