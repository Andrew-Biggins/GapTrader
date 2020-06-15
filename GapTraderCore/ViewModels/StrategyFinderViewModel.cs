using System;
using System.Threading;
using System.Windows.Input;
using Foundations;
using GapTraderCore.Interfaces;
using GapTraderCore.StrategyTesters;

namespace GapTraderCore.ViewModels
{
    public abstract class StrategyFinderViewModel : BindableBase
    {
        public EventHandler? StartSearchEventHandler;

        public IGapFillStrategy SelectedStrategy
        {
            get => _selectedStrategy;
            set => SetProperty(ref _selectedStrategy, value);
        }

        public ILoadable VariableSelector
        {
            get => _variableSelector;
            set => SetProperty(ref _variableSelector, value);
        }

        public bool SearchEnabled
        {
            get => _searchEnabled;
            protected set => SetProperty(ref _searchEnabled, value);
        }

        public ICommand FindStrategiesCommand => new BasicCommand(StartStrategySearch);

        protected StrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market, IRunner runner, AccountSizerViewModel accountSizer)
        {
            Runner = runner;
            StrategyTester = strategyTester;
            Market = market;
            AccountSizer = accountSizer;
            Market.PropertyChanged += (s, e) => CheckDataExists();
            CheckDataExists();
        }

        public void UpdateTester(GapFillStrategyTester tester)
        {
            StrategyTester = tester;
        }

        public abstract void FindStrategies(StrategyTestFilters filters, TradeDirection tradeDirection);

        protected void FinishSearch()
        {
            SearchEnabled = true;
            LoadingBar.Progress = 0;
            StrategyTester.ResetLevels();
        }

        protected virtual void StartStrategySearch()
        {
            VariableSelector = LoadingBar;
            SearchEnabled = false;

            // Raise the event to start the search on a separate thread to allow the UI to update  
            new Thread(() =>
            {
                //Thread.CurrentThread.IsBackground = true;
                StartSearchEventHandler.Raise(this);
            }).Start();
        }

        protected (int, int, int, int) SetFibIndexes()
        {
            var entryStartIndex = FibonacciServices.FirstFibRetraceIndex;
            var entryEndIndex = FibonacciServices.LastFibRetraceIndex;
            var targetStartIndex = FibonacciServices.FirstFibExtensionIndex;
            var targetEndIndex = FibonacciServices.LastFibExtensionIndex;

            if (StrategyTester is IntoGapStrategyTester)
            {
                entryStartIndex = FibonacciServices.FirstFibExtensionIndex;
                entryEndIndex = FibonacciServices.LastFibExtensionIndex;
                targetStartIndex = FibonacciServices.FirstFibRetraceIndex;
                targetEndIndex = FibonacciServices.LastFibRetraceIndex;
            }

            return (entryStartIndex, entryEndIndex, targetStartIndex, targetEndIndex);
        }

        private void CheckDataExists()
        {
            if (Market.DailyCandles.Count != 0)
            {
                SearchEnabled = true;
            }
        }

        protected readonly IRunner Runner;
        protected readonly IMarket Market;
        protected GapFillStrategyTester StrategyTester;
        protected readonly LoadingBarViewModel LoadingBar = new LoadingBarViewModel();
        protected AccountSizerViewModel AccountSizer;

        private ILoadable _variableSelector;
        private bool _searchEnabled;
        private IGapFillStrategy _selectedStrategy;
    }
}
