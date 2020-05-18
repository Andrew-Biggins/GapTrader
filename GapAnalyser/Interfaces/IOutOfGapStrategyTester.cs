namespace GapAnalyser.Interfaces
{
    public interface IOutOfGapStrategyTester : IStrategyTester
    {
        FibonacciLevel FibRetraceEntry { get; }

        double Stop { get; }

        double PointsTarget { get; }
    }
}