using GapAnalyser.Candles;
using GapAnalyser.Interfaces;
using GapAnalyser.Strategies;

namespace GapAnalyser.StrategyTesters
{
    public sealed class IntoGapStrategyTester : GapFillStrategyTester
    {
        public IntoGapStrategyTester(ITradeLevelCalculator tradeLevelCalculator) : base(
            tradeLevelCalculator)
        {
            FibLevelEntry = FibonacciLevel.OneHundredAndTwentySevenPointOne;
            FibLevelTarget = FibonacciLevel.FivePointNine;
        }

        protected override void UpdateFibLevelEntry(bool isFib)
        {
            FibLevelEntry = isFib
                ? FibonacciLevel.OneHundredAndTwentySevenPointOne
                : 0;
            RaisePropertyChanged(nameof(FibLevelEntry));
        }

        protected override void UpdateFibLevelTarget(bool isFib)
        {
            FibLevelTarget = isFib
                ? FibonacciLevel.FivePointNine
                : 0;
            RaisePropertyChanged(nameof(FibLevelTarget));
        }

        protected override void NewStrategy<TEntry, TTarget>(object entry, object target)
        {
            Strategy = new IntoGapStrategy<TEntry, TTarget>(entry, Stop, target, Stats);
        }

        protected override (double, double, double) CalculateTradeLevels(DailyCandle candle)
        {
            var gap = candle.Gap.GapPoints;
            var open = candle.Open;

            return GetTradeLevels(gap, open);
        }
    }
}
