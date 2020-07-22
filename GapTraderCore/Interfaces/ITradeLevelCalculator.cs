using TradingSharedCore;

namespace GapTraderCore.Interfaces
{
    public interface ITradeLevelCalculator
    {
        double GetEntryLevel(double gap, double open, double pointsEntry, bool fibEntry, FibonacciLevel fibLevelEntry);

        double GetFixedPointsStopLevel(double gap, double entry, double stopSize);

        double GetGapPercentageStopLevel(double gap, double entry, double stopPercentage);

        double GetTargetLevel(double gap, double entry, double open, double pointsTarget, bool fibTarget,
            FibonacciLevel fibLevelTarget);
    }
}