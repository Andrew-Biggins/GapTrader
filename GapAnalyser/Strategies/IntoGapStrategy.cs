using GapAnalyser.Interfaces;

namespace GapAnalyser.Strategies
{
    internal class IntoGapStrategy<TEntry, TTarget> : Strategy<TEntry, TTarget>
    {
        public IntoGapStrategy(object entry, double stop, object target, StrategyStats stats) : base(entry, stop, target, stats)
        {
        }
    }
}