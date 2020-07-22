using System.Collections.Generic;
using GapTraderCore.Interfaces;

namespace TradingSharedCore.Strategies
{
    public abstract class GapFillStrategy<TEntry, TTarget> : Strategy<TEntry, TTarget>
    { 
        public double MinimumGapSize { get; }

        protected GapFillStrategy(object entry, double stop, object target, StrategyStats stats, List<ITrade> trades,
            bool isFixedStop, double minimumGapSize, bool isStopTrailed, double trailedStopSize) : 
            base(entry, stop, target, stats, trades, isStopTrailed, trailedStopSize)
        {
            MinimumGapSize = minimumGapSize;
            SetName(entry, stop, target, isFixedStop, isStopTrailed, trailedStopSize);
        }

        protected GapFillStrategy(object entry, double stop, object target, bool isFixedStop, double minimumGapSize,
            bool isStopTrailed, double trailedStopSize) :
            base(entry, stop, target, isStopTrailed, trailedStopSize)
        {
            MinimumGapSize = minimumGapSize;
            SetName(entry,  stop,  target,  isFixedStop, isStopTrailed, trailedStopSize);
        }

        protected void SetName(object entry, double stop, object target, bool isFixedStop, bool isStopTrailed, double trailedStopSize)
        {
            var e = entry is FibonacciLevel ? $"{(double)(int)entry / 10}%" : $"{(double)entry}pts";
            var t = target is FibonacciLevel ? $"{(double)(int)target / 10}%" : $"{(double)target}pts";
            var stopSize = isFixedStop ? $"{stop}pts" : $"{stop}%";

            if (isStopTrailed)
            {
                Name = $"Entry: {e} | Target: {t} | Stop: {stopSize}, trailed by {trailedStopSize}pts | Min Gap Size: {MinimumGapSize}pts";
                ShortName = $"E: {e} | T: {t} | S: {stopSize} / {trailedStopSize}pts | MGS: {MinimumGapSize}pts";
            }
            else
            {
                Name = $"Entry: {e} | Target: {t} | Stop: {stopSize} | Min Gap Size: {MinimumGapSize}pts";
                ShortName = $"E: {e} | T: {t} | S: {stopSize} | MGS: {MinimumGapSize}pts";
            }
            
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
