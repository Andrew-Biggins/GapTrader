using System;
using System.Collections.Generic;
using System.Text;
using GapAnalyser.Interfaces;

namespace GapAnalyser.Strategies
{
    internal sealed class OutOfGapStrategy<TEntry, TTarget> : Strategy<TEntry, TTarget>
    {
        public OutOfGapStrategy(object entry, double stop, object target, StrategyStats stats) : base(entry, stop, target, stats)
        {
        }
    }
}
