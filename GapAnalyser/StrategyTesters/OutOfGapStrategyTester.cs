using GapAnalyser.Candles;
using GapAnalyser.Interfaces;
using GapAnalyser.Strategies;

namespace GapAnalyser.StrategyTesters
{
    public sealed class OutOfGapStrategyTester : GapFillStrategyTester
    {
        public OutOfGapStrategyTester(ITradeLevelCalculator tradeLevelCalculator) : base(
            tradeLevelCalculator)
        {
            FibLevelEntry = FibonacciLevel.FivePointNine;
            FibLevelTarget = FibonacciLevel.OneHundredAndTwentySevenPointOne;
        }

        public override void ResetLevels()
        {
            FibLevelTarget = FibonacciLevel.OneHundredAndTwentySevenPointOne;
            FibLevelEntry = FibonacciLevel.FivePointNine;
            base.ResetLevels();
        }

        protected override void UpdateFibLevelEntry(bool isFib)
        {
            FibLevelEntry = isFib
                ? FibonacciLevel.FivePointNine
                : 0;
            RaisePropertyChanged(nameof(FibLevelEntry));
        }

        protected override void UpdateFibLevelTarget(bool isFib)
        {
            FibLevelTarget = isFib
                ? FibonacciLevel.OneHundredAndTwentySevenPointOne
                : 0;
            RaisePropertyChanged(nameof(FibLevelTarget));
        }

        protected override void NewStrategy<TEntry, TTarget>(object entry, object target)
        {
            Strategy = new OutOfGapStrategy<TEntry, TTarget>(entry, Stop, target, Stats, MinimumGapSize);
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
