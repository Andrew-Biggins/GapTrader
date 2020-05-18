using Foundations;
using GapAnalyser.Interfaces;
using GapAnalyser.StrategyTesters;
using GapAnalyser.TradeCalculators;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace GapAnalyser.ViewModels
{
    public abstract class StrategyTesterViewModel : BindableBase
    {
        public GapFillStrategyTester StrategyTester
        {
            get => _strategyTester;
            set => SetProperty(ref _strategyTester, value);
        }

        public StrategyResultsStatsViewModel StrategyResultsStatsViewModel
        {
            get => _strategyResultsStatsViewModel;
            set => SetProperty(ref _strategyResultsStatsViewModel, value);
        }

        public StrategyFinderViewModel StrategyFinderViewModel
        {
            get => _strategyFinderViewModel;
            set => SetProperty(ref _strategyFinderViewModel, value);
        }

        public DateTime TestStartDate
        {
            get => _testStartDate;
            set => SetProperty(ref _testStartDate, value);
        }

        public DateTime TestEndDate
        {
            get => _testEndDate;
            set => SetProperty(ref _testEndDate, value);
        }

        public DateTime TestStartTime
        {
            get => _testStartTime;
            set => SetProperty(ref _testStartTime, value);
        }

        public DateTime TestEndTime
        {

            get => _testEndTime;
            set => SetProperty(ref _testEndTime, value);
        }

        public bool CanCalculate
        {
            get => _canCalculate;
            set => SetProperty(ref _canCalculate, value, nameof(CanCalculate));
        }

        public IMarket Market { get; }

        public ICommand TestStrategyCommand => new BasicCommand(TestStrategy);

        public StrategyTesterViewModel() { }

        protected StrategyTesterViewModel(IMarket market)
        {
            Market = market;
            StrategyTester = new OutOfGapStrategyTester(new GapTradeLevelCalculator());
            StrategyFinderViewModel = new StrategyFinderViewModel(StrategyTester, Market);
            UpdateFilters();
        }

        private void UpdateFilters()
        {
            if (Market.UkData)
            {
                TestStartTime = new DateTime(1, 1, 1, 8, 00, 00);
                TestEndTime = new DateTime(1, 1, 1, 16, 30, 00);
            }

            TestStartDate = Market.DataDetails.StartDate;
            TestEndDate = Market.DataDetails.EndDate;
        }

        protected abstract void TestStrategy();

        private StrategyResultsStatsViewModel _strategyResultsStatsViewModel = new StrategyResultsStatsViewModel();
        private GapFillStrategyTester _strategyTester;
        private DateTime _testStartDate;
        private DateTime _testEndDate;
        private DateTime _testStartTime = new DateTime(1, 1, 1, 14, 30, 00);
        private DateTime _testEndTime = new DateTime(1, 1, 1, 21, 00, 00);
        private bool _canCalculate;
        private StrategyFinderViewModel _strategyFinderViewModel;
    }
}
