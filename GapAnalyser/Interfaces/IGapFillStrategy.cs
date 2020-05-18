namespace GapAnalyser.Interfaces
{
    public interface IGapFillStrategy : IStrategy
    {
        FibonacciLevel FibEntry { get; }

        FibonacciLevel FibTarget { get; }
    }
}