using System;
using System.Drawing;
using Foundations.Optional;
using GapTraderCore.Interfaces;

namespace GapTraderCore.Trades
{
    public class Trade : ITrade
    {
        public double StrategyEntryLevel { get; }

        public double StopLevel { get; }

        public double StopSize { get; }

        public double Target { get; }

        public FibonacciLevel TargetFibLevel { get; set; }

        public double OpenLevel { get; }

        public FibonacciLevel OpenFibLevel { get; set; }

        public Optional<double> CloseLevel { get; }

        public DateTime OpenTime { get; }

        public Optional<DateTime> CloseTime { get; }

        public Optional<double> PointsProfit { get; private set; } = Option.None<double>();

        public double CashProfit { get; private set; }

        public TradeDirection Direction { get; }

        public double EntrySlippage { get; }

        public double Size { get; }

        public double RiskRewardRatio { get; }

        public Optional<double> ResultInR { get; private set; } = Option.None<double>();

        public Trade(double strategyEntry, double stop, double target, double openLevel, Optional<double> closeLevel, DateTime openTime,
            Optional<DateTime> closeTime, double size)
        {
            Direction = target > stop ? TradeDirection.Long : TradeDirection.Short;

            StrategyEntryLevel = strategyEntry;
            StopLevel = stop;
            StopSize = Math.Abs(openLevel - stop);
            Target = target;
            OpenLevel = openLevel;
            CloseLevel = closeLevel;
            OpenTime = openTime;
            CloseTime = closeTime;
            Size = size;
            RiskRewardRatio = Math.Abs(Target - OpenLevel) / StopSize;
            EntrySlippage = Direction == TradeDirection.Long
                ? strategyEntry - openLevel
                : openLevel - strategyEntry;

            CloseLevel.IfExistsThen(x =>
            {
                ResultInR = Option.Some(Math.Abs(x - OpenLevel) / StopSize);
                if (Direction == TradeDirection.Long)
                {
                    PointsProfit = Option.Some(x - openLevel);
                }
                else
                {
                    PointsProfit = Option.Some(openLevel - x);
                }
            });

            PointsProfit.IfExistsThen(x => { CashProfit = Size * x; });

            OpenFibLevel = FibonacciLevel.FivePointNine;
            TargetFibLevel = FibonacciLevel.OneHundredAndTwentySevenPointOne;
        }
    }
}