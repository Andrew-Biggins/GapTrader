using System;
using System.Collections.Generic;
using System.Text;
using GapAnalyser.Interfaces;

namespace GapAnalyser.Strategies
{
    internal sealed class OutOfGapStrategy<TEntry, TTarget> : Strategy<TEntry, TTarget>, IGapFillStrategy
    {
        public double MinimumGapSize { get; }

        public OutOfGapStrategy(object entry, double stop, object target, StrategyStats stats, double minimumGapSize) : base(entry, stop, target, stats)
        {
            MinimumGapSize = minimumGapSize;
        }
    }
}
