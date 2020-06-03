using GapAnalyser.Interfaces;
using System.Collections.Generic;

namespace GapAnalyser.Strategies
{
    internal sealed class OutOfGapStrategy<TEntry, TTarget> : Strategy<TEntry, TTarget>, IGapFillStrategy
    {
        public double MinimumGapSize { get; }

        public OutOfGapStrategy(object entry, double stop, object target, StrategyStats stats, double minimumGapSize,
            List<ITrade> trades, string title) : base(entry, stop, target, stats, trades, title)
        {
            MinimumGapSize = minimumGapSize;
        }
    }
}
