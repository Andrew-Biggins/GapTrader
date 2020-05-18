namespace GapAnalyser.Interfaces
{
    public interface ITradeLevelCalculator
    {
        double GetEntryLevel(double gap, double open, double pointsEntry, bool fibEntry, FibonacciLevel fibLevelEntry);

        double GetStopLevel(double gap, double entry, double stop);

        double GetTargetLevel(double gap, double entry, double open, double pointsTarget, bool fibTarget,
            FibonacciLevel fibLevelTarget);
    }
}