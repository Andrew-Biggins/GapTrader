using Foundations;
using GapAnalyser.Interfaces;
using GapAnalyser.StrategyTesters;
using System;
using System.Threading;
using System.Windows.Input;

namespace GapAnalyser.ViewModels
{
    public abstract class StrategyFinderViewModel : BindableBase
    {
        public EventHandler? StartSearchEventHandler;

        public IGapFillStrategy SelectedStrategy
        {
            get => _selectedStrategy;
            set => SetProperty(ref _selectedStrategy, value);
        }

        public BindableBase VariableSelector
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

        protected StrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market, IRunner runner)
        {
            Runner = runner;
            StrategyTester = strategyTester;
            Market = market;

            if (Market.DailyCandles.Count != 0)
            {
                SearchEnabled = true;
            }
        }

        public void UpdateTester(GapFillStrategyTester tester)
        {
            StrategyTester = tester;
        }

        public abstract void FindStrategies(StrategyTestFilters filters);

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

        protected readonly IRunner Runner;
        protected readonly IMarket Market;
        protected GapFillStrategyTester StrategyTester;
        protected readonly LoadingBar LoadingBar = new LoadingBar();

        private BindableBase _variableSelector;
        private bool _searchEnabled;
        private IGapFillStrategy _selectedStrategy;
    }
}
