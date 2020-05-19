using GapAnalyser.Interfaces;

namespace GapAnalyser.Strategies
{
    internal class IntoGapStrategy<TEntry, TTarget> : Strategy<TEntry, TTarget>, IGapFillStrategy
    {
        public double MinimumGapSize { get; }

        public IntoGapStrategy(object entry, double stop, object target, StrategyStats stats, double minimumGapSize) : base(entry, stop, target, stats)
        {
            MinimumGapSize = minimumGapSize;
        }
    }
}