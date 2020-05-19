namespace GapAnalyser.Interfaces
{
    public interface IGapFillStrategy : IStrategy
    {
        double MinimumGapSize { get; }
    }
}