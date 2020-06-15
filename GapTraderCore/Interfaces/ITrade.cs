using System;
using Foundations.Optional;

namespace GapTraderCore.Interfaces
{
    public interface ITrade
    {
        double StrategyEntryLevel { get; }

        double StopLevel { get; }

        double StopSize { get; }

        double Target { get; }

        double OpenLevel { get; }

        Optional<double> CloseLevel { get; }

        DateTime OpenTime { get; }

        Optional<DateTime> CloseTime { get; }

        Optional<double> PointsProfit { get; }

        double CashProfit { get; }

        FibonacciLevel TargetFibLevel { get; set; }

        FibonacciLevel OpenFibLevel { get; set; }

        TradeDirection Direction { get; }

        double EntrySlippage { get; }

        double Size { get; }
    }
}