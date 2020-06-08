using System;
using GapTraderCore.Interfaces;

namespace GapTraderCore.Trades
{
    public struct Trade : ITrade
    {
        public double StopLevel { get; }

        public double StopSize { get; }

        public double Target { get; }

        public FibonacciLevel TargetFibLevel { get; set; }

        public double OpenLevel { get; }

        public FibonacciLevel OpenFibLevel { get; set; }

        public double CloseLevel { get; }

        public DateTime OpenTime { get; }

        public DateTime CloseTime { get; }

        public double PointsProfit { get; }

        public double CashProfit { get; private set; }

        public double WinProbability { get; set; }

        public TradeDirection Direction { get; }

        public double EntrySlippage { get; }

        public Trade(double stop, double target, double openLevel, double closeLevel, DateTime openTime,
            DateTime closeTime, double pointsProfit, double entrySlippage, TradeDirection direction)
        {
            StopLevel = stop;
            StopSize = Math.Abs(openLevel - stop);
            Target = target;
            OpenLevel = openLevel;
            CloseLevel = closeLevel;
            OpenTime = openTime;
            CloseTime = closeTime;
            PointsProfit = pointsProfit;
            WinProbability = 0;
            CashProfit = 0;
            Direction = direction;
            EntrySlippage = entrySlippage;

            OpenFibLevel = FibonacciLevel.FivePointNine;
            TargetFibLevel = FibonacciLevel.OneHundredAndTwentySevenPointOne;
        }

        public void AddProfit(double size)
        {
            CashProfit = size * PointsProfit;
        }
    }
}