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
        public EventHandler StartSearchEventHandler;
        public EventHandler DataInUseToggle;

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

        public bool IsSearching => StrategyTester.IsSearching;

        public ICommand FindStrategiesCommand => new BasicCommand(StartStrategySearch);

        protected StrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market, IRunner runner,
            AccountSizerViewModel accountSizer)
        {
            Runner = runner;
            StrategyTester = strategyTester;
            Market = market;
            AccountSizer = accountSizer;
            AccountSizer.PropertyChanged += OnAccountSizePropertyChanged;
            Market.PropertyChanged += (s, e) => ClearSearchResults();
            VerifyInputs();
        }

        private void OnAccountSizePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AccountSizer.AccountStartSizeHasError) ||
                e.PropertyName == nameof(AccountSizer.RiskPercentageHasError))
            {
                VerifyInputs();
            }
        }

        public virtual void UpdateTester(GapFillStrategyTester tester)
        {
            StrategyTester = tester;
        }

        public virtual void ClearSearchResults()
        {
            VerifyInputs();
        }

        public abstract void FindStrategies(StrategyTestFilters filters, TradeDirection tradeDirection);

        protected void FinishSearch()
        {
            LoadingBar.Progress = 0;
            StrategyTester.ResetLevels();
            StrategyTester.IsSearching = false;
            RaisePropertyChanged(nameof(IsSearching));
            DataInUseToggle.Raise(this);
        }

        protected virtual void StartStrategySearch()
        {
            VariableSelector = LoadingBar;
            StrategyTester.IsSearching = true;
            RaisePropertyChanged(nameof(IsSearching));
            DataInUseToggle.Raise(this);

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

        private void VerifyInputs()
        {
            if (Market.DailyCandles.Count == 0 || AccountSizer.AccountStartSizeHasError ||
                AccountSizer.RiskPercentageHasError)
            {
                SearchEnabled = false;
            }
            else
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
