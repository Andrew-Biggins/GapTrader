using System;

namespace GapAnalyser.Interfaces
{
    public interface ITrade
    {
        double StopLevel { get; }

        double StopSize { get; }

        double Target { get; }

        double OpenLevel { get; }

        double CloseLevel { get; }

        DateTime OpenTime { get; }

        DateTime CloseTime { get; }

        double PointsProfit { get; }

        double CashProfit { get; }

        void AddProfit(double size);

        FibonacciLevel TargetFibLevel { get; set; }

        FibonacciLevel OpenFibLevel { get; set; }

        double WinProbability { get; set; }
    }
}