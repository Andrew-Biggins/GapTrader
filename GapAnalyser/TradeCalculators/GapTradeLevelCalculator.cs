using GapAnalyser.Interfaces;
using static GapAnalyser.TradeServices;

namespace GapAnalyser.TradeCalculators
{
    internal sealed class GapTradeLevelCalculator : ITradeLevelCalculator
    {
        public double GetEntryLevel(double gap, double open, double pointsEntry, bool fibEntry, FibonacciLevel fibLevelEntry)
        {
            return GetGapTradeEntryLevel(gap, open, pointsEntry, fibEntry, fibLevelEntry);
        }

        public double GetStopLevel(double gap, double entry, double stop)
        {
            return GetGapTradeStopLevel(gap, entry, stop);
        }

        public double GetTargetLevel(double gap, double entry, double open, double pointsTarget, bool fibTarget, FibonacciLevel fibLevelTarget)
        {
            return GetGapTradeTargetLevel(gap, entry, open, pointsTarget, fibTarget, fibLevelTarget);
        }
    }
}
