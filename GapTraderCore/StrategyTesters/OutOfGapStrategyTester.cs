﻿using GapTraderCore.Candles;
using GapTraderCore.Interfaces;
using TradingSharedCore;
using TradingSharedCore.Strategies;

namespace GapTraderCore.StrategyTesters
{
    public sealed class OutOfGapStrategyTester : GapFillStrategyTester
    {
        public OutOfGapStrategyTester(ITradeLevelCalculator tradeLevelCalculator) : base(
            tradeLevelCalculator)
        {
            DefaultFibEntry = FibonacciLevel.FivePointNine;
            DefaultFibTarget = FibonacciLevel.OneHundredAndTwentySevenPointOne;
            FibLevelEntry = DefaultFibEntry;
            FibLevelTarget = DefaultFibTarget;
        }

        protected override void NewStrategy<TEntry, TTarget>(object entry, object target)
        {
            Strategy = new OutOfGapStrategy<TEntry, TTarget>(entry, Stop, target, Stats, MinimumGapSize, Trades, IsFixedStop, IsStopTrailed, TrailedStopSize);
        }

        protected override (double, double, double) CalculateTradeLevels(DailyCandle candle)
        {
            // Invert gap to get the correct trade levels for out of gap trade direction
            var gap = candle.Gap.GapPoints * -1;
            var open = candle.Open;

            return GetTradeLevels(gap, open);
        }
    }
}
