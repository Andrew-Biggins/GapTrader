using System;
using System.Collections.Generic;
using System.Linq;
using GapAnalyser.Interfaces;

namespace GapAnalyser.StrategyTesters
{
    public abstract class GapFillStrategyTester : StrategyTester
    {
        public IGapFillStrategy Strategy { get; protected set; }

        public FibonacciLevel FibLevelEntry { get; set; }

        public double PointsEntry
        {
            get => _pointsEntry;
            set
            {
                _pointsEntry = value;
                RaisePropertyChanged(nameof(PointsEntry));
            }
        }

        public double Stop { get; set; } = DefaultStopSize;

        public FibonacciLevel FibLevelTarget { get; set; }

        public double PointsTarget
        {
            get => _pointsTarget;
            set
            {
                _pointsTarget = value;
                RaisePropertyChanged(nameof(PointsTarget));
            }
        }

        public bool FibTarget
        {
            get => _fibTarget;
            set
            {
                UpdateFibLevelTarget(value);
                UpdatePointsTarget(value);
                _fibTarget = value;
                RaisePropertyChanged(nameof(FibTarget));
            }
        }

        public bool FibEntry
        {
            get => _fibEntry;
            set
            {
                UpdateFibLevelEntry(value);
                UpdatePointsEntry(value);
                _fibEntry = value;
                RaisePropertyChanged(nameof(FibEntry));
            }
        }

        public bool TargetHasError
        {
            get => _targetHasError;
            set => SetProperty(ref _targetHasError, value, nameof(TargetHasError));
        }

        public bool StopHasError
        {
            get => _stopHasError;
            set => SetProperty(ref _stopHasError, value, nameof(StopHasError));
        }

        public bool EntryHasError
        {
            get => _entryHasError;
            set => SetProperty(ref _entryHasError, value, nameof(EntryHasError));
        }

        protected GapFillStrategyTester(ITradeLevelCalculator tradeLevelCalculator) : base(
            tradeLevelCalculator)
        {
        }

        public void ResetLevels()
        {
            Stop = DefaultStopSize;
            FibLevelEntry = DefaultFibEntry;
            FibLevelTarget = DefaultFibTarget;
            RaisePropertyChanged(nameof(Stop));
            RaisePropertyChanged(nameof(FibLevelTarget));
            RaisePropertyChanged(nameof(FibLevelEntry));
        }

        public void TestStrategy(IMarket market, StrategyTestFilters filters, double minimumGapSize,
            double maximumGapSize = double.PositiveInfinity)
        {
            MinimumGapSize = minimumGapSize;

            foreach (var candle in market.DailyCandles.Where(candle =>
                candle.Gap.AbsoluteGapPoints >= MinimumGapSize && candle.Gap.AbsoluteGapPoints <= maximumGapSize))
            {
                if (candle.Date.Date >= filters.StartDate.Date && candle.Date.Date <= filters.EndDate.Date)
                {
                    CompareMinuteData(candle, market.MinuteData, filters.StartTime, filters.EndTime);
                }
            }

            FinalizeStats();
            CreateNewStrategy();
            Trades = new List<ITrade>();
        }

        protected abstract void NewStrategy<TEntry, TTarget>(object entry, object target, string title = "");

        protected void CreateNewStrategy()
        {
            if (FibTarget)
            {
                if (FibEntry)
                {
                    var entry = (double) FibLevelEntry / 10;
                    var target = (double) FibLevelTarget / 10;
                    var title =
                        $"Entry: {entry}% | Target: {target}% | Stop Size: {Stop}pts | Min Gap Size: {MinimumGapSize}pts";
                    NewStrategy<FibonacciLevel, FibonacciLevel>(FibLevelEntry, FibLevelTarget, title);
                }
                else
                {
                    NewStrategy<double, FibonacciLevel>(PointsEntry, FibLevelTarget);
                }
            }
            else
            {
                if (FibEntry)
                {
                    NewStrategy<FibonacciLevel, double>(FibLevelEntry, PointsTarget);
                }
                else
                {
                    NewStrategy<double, double>(PointsEntry, PointsTarget);
                }
            }
        }

        protected (double, double, double, double) GetTradeLevels(double gap, double open)
        {
            var entry = TradeLevelCalculator.GetEntryLevel(gap, open, PointsEntry, FibEntry, FibLevelEntry);
            var stopLevel = TradeLevelCalculator.GetStopLevel(gap, entry, Stop);
            var target = TradeLevelCalculator.GetTargetLevel(gap, entry, open, PointsTarget, FibTarget, FibLevelTarget);

            return (entry, stopLevel, target, Stop);
        }

        private void UpdateFibLevelEntry(bool isFib)
        {
            FibLevelEntry = isFib
                ? DefaultFibEntry
                : 0;
            RaisePropertyChanged(nameof(FibLevelEntry));
        }

        private void UpdateFibLevelTarget(bool isFib)
        {
            FibLevelTarget = isFib
                ? DefaultFibTarget
                : 0;
            RaisePropertyChanged(nameof(FibLevelTarget));
        }

        private void UpdatePointsTarget(bool isFib)
        {
            PointsTarget = isFib ? 0 : 50;
            RaisePropertyChanged(nameof(PointsTarget));
        }

        private void UpdatePointsEntry(bool isFib)
        {
            PointsEntry = isFib ? 0 : 50;
            RaisePropertyChanged(nameof(PointsEntry));
        }

        protected double MinimumGapSize;
        protected FibonacciLevel DefaultFibEntry;
        protected FibonacciLevel DefaultFibTarget;

        private const double DefaultStopSize = 100;

        private double _pointsEntry;
        private double _pointsTarget;
        private bool _fibEntry = true;
        private bool _fibTarget = true;
        private bool _targetHasError;
        private bool _stopHasError;
        private bool _entryHasError;
    }
}
