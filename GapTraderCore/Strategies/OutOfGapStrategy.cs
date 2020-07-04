using System.Collections.Generic;
using GapTraderCore.Interfaces;
using static GapTraderCore.Strings;

namespace GapTraderCore.Strategies
{
    internal sealed class OutOfGapStrategy<TEntry, TTarget> : GapFillStrategy<TEntry, TTarget>, IGapFillStrategy
    {
        public OutOfGapStrategy(object entry, double stop, object target, StrategyStats stats, double minimumGapSize,
            List<ITrade> trades, bool isFixedStop, bool isStopTrailed, double trailedStopSize) : base(entry, stop, target, stats, trades, isFixedStop, minimumGapSize, isStopTrailed, trailedStopSize)
        {
            Name = $"{OutOfGapName} | {Name}";
            ShortName = $"{OutOfGapName} | {ShortName}";
        }

        public OutOfGapStrategy(object entry, double stop, object target, double minimumGapSize,
            bool isFixedStop, bool isStopTrailed, double trailedStopSize) : base(entry, stop, target, isFixedStop, minimumGapSize, isStopTrailed, trailedStopSize)
        {
            Name = $"{OutOfGapName} | {Name}";
            ShortName = $"{OutOfGapName} | {ShortName}";
        }
    }
}
