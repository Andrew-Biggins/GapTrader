﻿using System;
using System.Windows.Input;
using GapTraderCore.Interfaces;
using GapTraderCore.StrategyTesters;
using GapTraderCore.TradeCalculators;
using TradingSharedCore;
using TradingSharedCore.Interfaces;
using TradingSharedCore.ViewModels;

namespace GapTraderCore.ViewModels
{
    public abstract class StrategyTesterViewModel : BindableBase
    {
        public GapFillStrategyTester StrategyTester
        {
            get => _strategyTester;
            set => SetProperty(ref _strategyTester, value);
        }

        public GapTraderStrategyResultsStatsViewModel StrategyResultsStatsViewModel
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

        public TradeDirection TradeDirection { get; set; } = TradeDirection.Both;

        public bool IsDynamicTest
        {
            get => _isDynamicTest;
            set
            {
                if (_isDynamicTest != value)
                {
                    _isDynamicTest = value;
                    SwitchStrategyFinder();
                }
            }
        }

        public AccountSizerViewModel AccountSizer { get; } = new AccountSizerViewModel();

        public IMarket Market { get; }

        public ICommand TestStrategyCommand => new BasicCommand(TestStrategy);

        protected StrategyTesterViewModel(IMarket market, IGapTraderRunner runner)
        {
            Market = market;
            Runner = runner;
            StrategyTester = new OutOfGapStrategyTester(new GapTradeLevelCalculator());
            StrategyFinderViewModel = new StaticStrategyFinderViewModel(StrategyTester, Market, Runner, AccountSizer);
            UpdateFilters();
            StrategyFinderViewModel.StartSearchEventHandler += FindStrategies;
        }

        protected abstract void TestStrategy();

        private void FindStrategies(object sender, EventArgs e)
        {
            var filters = new StrategyTestFilters(TestStartDate, TestEndDate, TestStartTime.TimeOfDay,
                TestEndTime.TimeOfDay);
            StrategyFinderViewModel.FindStrategies(filters, TradeDirection);
        }

        protected void UpdateFilters()
        {
            TestStartTime = new DateTime(1, 1, 1, Market.DataDetails.OpenTime.Hours,
                Market.DataDetails.OpenTime.Minutes, Market.DataDetails.OpenTime.Seconds);
            TestEndTime = new DateTime(1, 1, 1, Market.DataDetails.CloseTime.Hours,
                Market.DataDetails.CloseTime.Minutes, Market.DataDetails.CloseTime.Seconds);
            TestStartDate = Market.DataDetails.StartDate;
            TestEndDate = Market.DataDetails.EndDate;
        }

        private void SwitchStrategyFinder()
        {
            if (_isDynamicTest)
            {
                StrategyFinderViewModel =
                    new DynamicStrategyFinderViewModel(StrategyTester, Market, Runner, AccountSizer);
            }
            else
            {
                StrategyFinderViewModel =
                    new StaticStrategyFinderViewModel(StrategyTester, Market, Runner, AccountSizer);
            }

            StrategyFinderViewModel.StartSearchEventHandler += FindStrategies;
        }


        protected readonly IGapTraderRunner Runner;

        private GapTraderStrategyResultsStatsViewModel _strategyResultsStatsViewModel = new GapTraderStrategyResultsStatsViewModel();
        private GapFillStrategyTester _strategyTester;
        private DateTime _testStartDate;
        private DateTime _testEndDate;
        private DateTime _testStartTime;
        private DateTime _testEndTime;
        private bool _canCalculate;
        private StrategyFinderViewModel _strategyFinderViewModel;
        private bool _isDynamicTest;
    }
}
