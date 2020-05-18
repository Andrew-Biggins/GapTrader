using GapAnalyser.Interfaces;
using GapAnalyser.StrategyTesters;
using GapAnalyser.TradeCalculators;
using System.ComponentModel;

namespace GapAnalyser.ViewModels
{
    public sealed class GapFillStrategyTesterViewModel : StrategyTesterViewModel
    {
        public bool MinimumGapHasError
        {
            get => _minimumGapHasError;
            set
            { 
                SetProperty(ref _minimumGapHasError, value, nameof(MinimumGapHasError));
                VerifyInputs();
            }
        }

        public double MinimumGapSize { get; set; } = 200;

        public bool TradeIntoGap
        {
            get => _tradeIntoGap;
            set
            {
                _tradeIntoGap = value;
                SwitchStrategyTester();
                RaisePropertyChanged(nameof(StrategyTester));
                StrategyTester.PropertyChanged += OnStrategyTesterDataChanged;
            }
        }

        public GapFillStrategyTesterViewModel()
        {
        }

        public GapFillStrategyTesterViewModel(IMarket market) : base(market)
        {
            StrategyTester.PropertyChanged += OnStrategyTesterDataChanged;
            VerifyInputs();
        }

        protected override void TestStrategy()
        {
            var filters = new StrategyTestFilters(TestStartDate, TestEndDate, TestStartTime.TimeOfDay,
                TestEndTime.TimeOfDay);

            StrategyTester.TestStrategy(Market, filters, MinimumGapSize);
            StrategyResultsStatsViewModel = new StrategyResultsStatsViewModel(StrategyTester.Strategy);
        }

        private void OnStrategyTesterDataChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StrategyTester.EntryHasError):
                case nameof(StrategyTester.TargetHasError):
                case nameof(StrategyTester.StopHasError):
                    VerifyInputs();
                    break;
            }
        }

        private void VerifyInputs()
        {
            var hasData = Market.DailyCandles.Count > 0;

            if (MinimumGapHasError || StrategyTester.EntryHasError || StrategyTester.StopHasError ||
                StrategyTester.TargetHasError || !hasData)
            {
                CanCalculate = false;
            }
            else
            {
                CanCalculate = true;
            }
        }

        private void SwitchStrategyTester() 
        {
            if (_tradeIntoGap)
            {
                StrategyTester = new IntoGapStrategyTester(new GapTradeLevelCalculator());
            }
            else
            {
                StrategyTester = new OutOfGapStrategyTester(new GapTradeLevelCalculator());
            }

            StrategyFinderViewModel.StrategyTester = StrategyTester;
        }

        private bool _tradeIntoGap;
        private bool _minimumGapHasError;
    }
}
