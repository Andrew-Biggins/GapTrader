namespace GapAnalyser.Interfaces
{
    public interface IIntoGapStrategyTester : IStrategyTester
    {
        double PointsEntry { get; }

        double Stop { get; }

        FibonacciLevel FibRetraceTarget { get; }

       // double Adjustment { get; }
    }
}