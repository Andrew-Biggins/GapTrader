using System.Collections.Generic;
using GapTraderCore.Interfaces;
using static GapTraderCore.Strings;

namespace GapTraderCore.Strategies
{
    internal class IntoGapStrategy<TEntry, TTarget> : GapFillStrategy<TEntry, TTarget>, IGapFillStrategy
    {
        public IntoGapStrategy(object entry, double stop, object target, StrategyStats stats, double minimumGapSize,
            List<ITrade> trades, bool isFixedStop) : base(entry, stop, target, stats, trades, isFixedStop, minimumGapSize)
        {
            Name = $"{IntoGapName} | {Name}";
            ShortName = $"{IntoGapName} | {ShortName}";
        }

        public IntoGapStrategy(object entry, double stop, object target, double minimumGapSize,
            bool isFixedStop) : base(entry, stop, target, isFixedStop, minimumGapSize)
        {
            Name = $"{IntoGapName} | {Name}";
            ShortName = $"{IntoGapName} | {ShortName}";
        }
    }
}