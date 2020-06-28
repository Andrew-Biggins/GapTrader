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

        public double StopSize { get; private set; }

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

        public double RiskRewardRatio { get; private set; }

        public Optional<double> ResultInR { get; private set; } = Option.None<double>();

        public Trade(double strategyEntry, double stop, double target, double openLevel, Optional<double> closeLevel,
            DateTime openTime, Optional<DateTime> closeTime, double size)
        {
            Direction = target > stop 
                ? TradeDirection.Long 
                : TradeDirection.Short;

            StrategyEntryLevel = strategyEntry;
            StopLevel = stop;
            Target = target;
            OpenLevel = openLevel;
            CloseLevel = closeLevel;
            OpenTime = openTime;
            CloseTime = closeTime;
            Size = size;
            EntrySlippage = Direction == TradeDirection.Long
                ? strategyEntry - openLevel
                : openLevel - strategyEntry;

            CalculateResult();
            //  OpenFibLevel = FibonacciLevel.FivePointNine;
          // //  TargetFibLevel = FibonacciLevel.OneHundredAndTwentySevenPointOne;
        }

        public Trade(SavedTrade savedTrade)
        {
            StrategyEntryLevel = savedTrade.StrategyEntryLevel;
            StopLevel = savedTrade.StopLevel;
            Target = savedTrade.Target;
            OpenLevel = savedTrade.OpenLevel;

            CloseLevel = savedTrade.CloseLevel == -1 
                ? Option.None<double>() 
                : Option.Some(savedTrade.CloseLevel);

            OpenTime = savedTrade.OpenTime;

            CloseTime = savedTrade.CloseTime == DateTime.MinValue
                ? Option.None<DateTime>()
                : Option.Some(savedTrade.CloseTime);

            Size = savedTrade.Size;

            CalculateResult();
        }

        private void CalculateResult()
        {
            StopSize = Math.Abs(OpenLevel - StopLevel);
            RiskRewardRatio = Math.Abs(Target - StrategyEntryLevel) / StopSize;
            
            CloseLevel.IfExistsThen(x =>
            {
                ResultInR = Option.Some(Math.Abs(x - OpenLevel) / StopSize);
                PointsProfit = Direction == TradeDirection.Long
                    ? Option.Some(x - OpenLevel)
                    : Option.Some(OpenLevel - x);
            });

            PointsProfit.IfExistsThen(x => { CashProfit = Size * x; });
        }
    }
}