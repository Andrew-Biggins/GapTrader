using System.Collections.Generic;
using System.Linq;
using GapTraderCore.Interfaces;

namespace GapTraderCore.StrategyTesters
{
    public abstract class GapFillStrategyTester : StrategyTester
    {
        public IGapFillStrategy Strategy { get; protected set; }

        public FibonacciLevel FibLevelEntry
        {
            get => _fibLevelEntry;
            set => SetProperty(ref _fibLevelEntry, value);
        }

        public double PointsEntry
        {
            get => _pointsEntry;
            set => SetProperty(ref _pointsEntry, value);
        }

        public double Stop { get; set; } = DefaultStopSize;

        public FibonacciLevel FibLevelTarget
        {
            get => _fibLevelTarget;
            set => SetProperty(ref _fibLevelTarget, value);
        }

        public double PointsTarget
        {
            get => _pointsTarget;
            set
            {
                _pointsTarget = value;
                RaisePropertyChanged(nameof(PointsTarget));
            }
        }

        public bool IsFibTarget
        {
            get => _isFibTarget;
            set
            {
                FibLevelTarget = value ? DefaultFibTarget : 0; 
                PointsTarget = value ? 0 : 50;
                SetProperty(ref _isFibTarget, value);
            }
        }

        public bool IsFibEntry
        {
            get => _isFibEntry;
            set
            {
                FibLevelEntry = value ? DefaultFibEntry : 0;
                PointsEntry = value ? 0 : 50;
                SetProperty(ref _isFibEntry, value);
            }
        }

        public bool TargetHasError
        {
            get => _targetHasError;
            set => SetProperty(ref _targetHasError, value, nameof(TargetHasError));
        }

        public bool StopSizeHasError
        {
            get => _stopSizeHasError;
            set => SetProperty(ref _stopSizeHasError, value, nameof(StopSizeHasError));
        }

        public bool StopTrailHasError
        {
            get => _stopTrailHasError;
            set => SetProperty(ref _stopTrailHasError, value, nameof(StopTrailHasError));
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

            CreateNewStrategy();
            Trades = new List<ITrade>();
        }

        protected abstract void NewStrategy<TEntry, TTarget>(object entry, object target);

        protected void CreateNewStrategy()
        {
            if (IsFibTarget)
            {
                if (IsFibEntry)
                {
                    NewStrategy<FibonacciLevel, FibonacciLevel>(FibLevelEntry, FibLevelTarget);
                }
                else
                {
                    NewStrategy<double, FibonacciLevel>(PointsEntry, FibLevelTarget);
                }
            }
            else
            {
                if (IsFibEntry)
                {
                    NewStrategy<FibonacciLevel, double>(FibLevelEntry, PointsTarget);
                }
                else
                {
                    NewStrategy<double, double>(PointsEntry, PointsTarget);
                }
            }
        }

        protected (double, double, double) GetTradeLevels(double gap, double open)
        {
            var entry = TradeLevelCalculator.GetEntryLevel(gap, open, PointsEntry, IsFibEntry, FibLevelEntry);

            var stopLevel = IsFixedStop
                ? TradeLevelCalculator.GetFixedPointsStopLevel(gap, entry, Stop)
                : TradeLevelCalculator.GetGapPercentageStopLevel(gap, entry, Stop);

            var target = TradeLevelCalculator.GetTargetLevel(gap, entry, open, PointsTarget, IsFibTarget, FibLevelTarget);

            return (entry, stopLevel, target);
        }

        protected double MinimumGapSize;
        protected FibonacciLevel DefaultFibEntry;
        protected FibonacciLevel DefaultFibTarget;

        private const double DefaultStopSize = 20;

        private FibonacciLevel _fibLevelEntry;
        private FibonacciLevel _fibLevelTarget;
        private double _pointsEntry;
        private double _pointsTarget;
        private bool _isFibEntry = true;
        private bool _isFibTarget = true;
        private bool _targetHasError;
        private bool _stopSizeHasError;
        private bool _entryHasError;
        private bool _stopTrailHasError;
    }
}
