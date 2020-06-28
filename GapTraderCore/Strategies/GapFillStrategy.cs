using System;
using System.Collections.Generic;
using System.Text;
using GapTraderCore.Interfaces;

namespace GapTraderCore.Strategies
{
    internal abstract class GapFillStrategy<TEntry, TTarget> : Strategy<TEntry, TTarget>
    { 
        public double MinimumGapSize { get; }

        protected GapFillStrategy(object entry, double stop, object target, StrategyStats stats, List<ITrade> trades, bool isFixedStop, double minimumGapSize) : base(entry, stop, target, stats, trades)
        {
            MinimumGapSize = minimumGapSize;
            SetName(entry, stop, target, isFixedStop);
        }

        protected GapFillStrategy(object entry, double stop, object target, bool isFixedStop, double minimumGapSize) : base(entry, stop, target)
        {
            MinimumGapSize = minimumGapSize;
            SetName(entry,  stop,  target,  isFixedStop);
        }

        protected void SetName(object entry, double stop, object target, bool isFixedStop)
        {
            var e = entry is FibonacciLevel ? $"{(double)(int)entry / 10}%" : $"{(double)entry}pts";
            var t = target is FibonacciLevel ? $"{(double)(int)target / 10}%" : $"{(double)target}pts";
            var s = isFixedStop ? $"{stop}pts" : $"{stop}%";
            Name = $"Entry: {e} | Target: {t} | Stop: {s} | Min Gap Size: {MinimumGapSize}pts";
            ShortName = $"E: {e} | T: {t} | S: {s} | MGS: {MinimumGapSize}pts";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
