using GapTraderCore.Candles;
using GapTraderCore.Interfaces;
using TradingSharedCore;
using TradingSharedCore.Strategies;

namespace GapTraderCore.StrategyTesters
{
    public sealed class IntoGapStrategyTester : GapFillStrategyTester
    {
        public IntoGapStrategyTester(ITradeLevelCalculator tradeLevelCalculator) : base(
            tradeLevelCalculator)
        {
            DefaultFibEntry = FibonacciLevel.OneHundredAndTwentySevenPointOne;
            DefaultFibTarget = FibonacciLevel.FivePointNine;
            FibLevelEntry = DefaultFibEntry;
            FibLevelTarget = DefaultFibTarget;
        }

        protected override void NewStrategy<TEntry, TTarget>(object entry, object target)
        {
            Strategy = new IntoGapStrategy<TEntry, TTarget>(entry, Stop, target, Stats, MinimumGapSize, Trades,
                IsFixedStop, IsStopTrailed, TrailedStopSize);
        }

        protected override (double, double, double) CalculateTradeLevels(DailyCandle candle)
        {
            var gap = candle.Gap.GapPoints;
            var open = candle.Open;

            return GetTradeLevels(gap, open);
        }
    }
}
