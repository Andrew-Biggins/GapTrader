using System;
using System.ComponentModel;
using GapTraderCore.Interfaces;
using GapTraderCore.StrategyTesters;
using GapTraderCore.TradeCalculators;
using TradingSharedCore.Interfaces;
using TradingSharedCore.ViewModels;

namespace GapTraderCore.ViewModels
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
                if (_tradeIntoGap != value)
                {
                    _tradeIntoGap = value;
                    SwitchStrategyTester();
                    StrategyTester.PropertyChanged += OnInputsChanged;
                }
            }
        }

        public GapFillStrategyTesterViewModel(IMarket market, IGapTraderRunner runner) : base(market, runner)
        {
            StrategyTester.PropertyChanged += OnInputsChanged;
            AccountSizer.PropertyChanged += OnInputsChanged;
            Market.PropertyChanged += OnMarketDataChanged;
            VerifyInputs();
        }

        protected override void TestStrategy()
        {
            var filters = new StrategyTestFilters(TestStartDate, TestEndDate, TestStartTime.TimeOfDay,
                TestEndTime.TimeOfDay);
            StrategyTester.IsStopTrailed = StrategyTester.IsStopTrailedForwarder;
            StrategyTester.SelectedDirection = TradeDirection;
            StrategyTester.SetSizing(AccountSizer.AccountStartSize, AccountSizer.RiskPercentage, AccountSizer.Compound);
            StrategyTester.TestStrategy(Market, filters, MinimumGapSize);
            StrategyResultsStatsViewModel = new GapTraderStrategyResultsStatsViewModel(StrategyTester.Strategy, Runner, AccountSizer.AccountStartSize);
        }

        private void OnMarketDataChanged(object sender, PropertyChangedEventArgs e)
        {
            VerifyInputs();
            UpdateFilters();
        }

        private void OnInputsChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(StrategyTester.EntryHasError):
                case nameof(StrategyTester.TargetHasError):
                case nameof(StrategyTester.StopSizeHasError):
                case nameof(StrategyTester.StopTrailHasError):
                case nameof(AccountSizer.AccountStartSizeHasError):
                case nameof(AccountSizer.RiskPercentageHasError):
                    VerifyInputs();
                    break;
            }
        }

        private void VerifyInputs()
        {
            var hasData = Market.DailyCandles.Count > 0;

            if (MinimumGapHasError || StrategyTester.EntryHasError || StrategyTester.StopSizeHasError ||
                StrategyTester.TargetHasError || StrategyTester.StopTrailHasError || AccountSizer.AccountStartSizeHasError ||
                AccountSizer.RiskPercentageHasError || !hasData)
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

            StrategyFinderViewModel.UpdateTester(StrategyTester);
            StrategyFinderViewModel.ClearSearchResults();
            StrategyResultsStatsViewModel = new GapTraderStrategyResultsStatsViewModel();
        }

        private bool _tradeIntoGap;
        private bool _minimumGapHasError;
    }
}
