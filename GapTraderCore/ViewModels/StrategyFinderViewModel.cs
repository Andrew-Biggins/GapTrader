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

       // public EventHandler? SearchCompleteEventHandler;

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

        protected StrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market, IRunner runner,
            AccountSizerViewModel accountSizer)
        {
            Runner = runner;
            StrategyTester = strategyTester;
            Market = market;
            AccountSizer = accountSizer;
            Market.PropertyChanged += (s, e) => CheckDataExists();
            CheckDataExists();
        }

        public virtual void UpdateTester(GapFillStrategyTester tester)
        {
            StrategyTester = tester;
        }

        public abstract void FindStrategies(StrategyTestFilters filters, TradeDirection tradeDirection);

        protected void FinishSearch()
        {
            SearchEnabled = true;
            LoadingBar.Progress = 0;
            StrategyTester.ResetLevels();
          //  SearchCompleteEventHandler.Raise(this);
            StrategyTester.IsSearching = false;
        }

        protected virtual void StartStrategySearch()
        {
            VariableSelector = LoadingBar;
            SearchEnabled = false;
            StrategyTester.IsSearching = true;

            // Raise the event to start the search on a separate thread to allow the UI to update  
            new Thread(() =>
            {
                //Thread.CurrentThread.IsBackground = true;
                StartSearchEventHandler.Raise(this);
            }).Start();
        }

        protected (int, int) SetEntryFibIndexes()
        {
            var entryStartIndex = FibonacciServices.FirstFibRetraceIndex;
            var entryEndIndex = FibonacciServices.LastFibRetraceIndex;
            
            if (StrategyTester is IntoGapStrategyTester)
            {
                entryStartIndex = FibonacciServices.FirstFibExtensionIndex;
                entryEndIndex = FibonacciServices.LastFibExtensionIndex;
            }

            return (entryStartIndex, entryEndIndex);
        }

        protected (int, int) SetTargetFibIndexes()
        {
            var targetStartIndex = FibonacciServices.FirstFibExtensionIndex;
            var targetEndIndex = FibonacciServices.LastFibExtensionIndex;

            if (StrategyTester is IntoGapStrategyTester)
            {
                targetStartIndex = FibonacciServices.FirstFibRetraceIndex;
                targetEndIndex = FibonacciServices.LastFibRetraceIndex;
            }

            return (targetStartIndex, targetEndIndex);
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
