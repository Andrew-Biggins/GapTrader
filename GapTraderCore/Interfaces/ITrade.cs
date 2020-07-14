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

        Optional<double> MaximumAdverseExcursionPoints { get; }

        Optional<double> MaximumAdverseExcursionPercentageOfStop { get; }

        DateTime OpenTime { get; }

        Optional<DateTime> CloseTime { get; }

        Optional<double> PointsProfit { get; }

        Optional<double> CashProfit { get; }

        FibonacciLevel TargetFibLevel { get; set; }

        FibonacciLevel OpenFibLevel { get; set; }

        TradeDirection Direction { get; }

        double EntrySlippage { get; }

        double RiskRewardRatio { get; }

        Optional<double> ResultInR { get; }

        double Size { get; }

        Optional<double> MaximumFavourableExcursionPoints { get; }

        Optional<double> PointsProfitPercentageOfMaximumFavourableExcursion { get; }

        Optional<double> UnrealisedProfitPoints { get; }

        Optional<double> UnrealisedProfitCash { get; }
    }
}