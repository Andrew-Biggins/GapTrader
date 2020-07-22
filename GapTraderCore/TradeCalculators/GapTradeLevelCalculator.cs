using GapTraderCore.Interfaces;
using TradingSharedCore;
using static GapTraderCore.TradeServices;

namespace GapTraderCore.TradeCalculators
{
    internal sealed class GapTradeLevelCalculator : ITradeLevelCalculator
    {
        public double GetEntryLevel(double gap, double open, double pointsEntry, bool fibEntry, FibonacciLevel fibLevelEntry)
        {
            return GetGapTradeEntryLevel(gap, open, pointsEntry, fibEntry, fibLevelEntry);
        }

        public double GetFixedPointsStopLevel(double gap, double entry, double stopSize)
        {
            return GetGapTradeFixedPointsStopLevel(gap, entry, stopSize);
        }

        public double GetGapPercentageStopLevel(double gap, double entry, double stopPercentage)
        {
            return GetGapTradePercentageStopLevel(gap, entry, stopPercentage);
        }

        public double GetTargetLevel(double gap, double entry, double open, double pointsTarget, bool fibTarget, FibonacciLevel fibLevelTarget)
        {
            return GetGapTradeTargetLevel(gap, entry, open, pointsTarget, fibTarget, fibLevelTarget);
        }
    }
}
