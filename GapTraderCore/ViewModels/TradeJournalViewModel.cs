using Foundations;
using Foundations.Optional;
using GapTraderCore.Interfaces;
using GapTraderCore.Strategies;
using GapTraderCore.Trades;
using GapTraderCore.ViewModelAdapters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Input;
using static GapTraderCore.DataProcessor;
using static System.IO.Directory;

namespace GapTraderCore.ViewModels
{
    public sealed class TradeJournalViewModel : BindableBase
    {
        public ITradePlot Plot { get; } = new TradePlot();

        public ObservableCollection<IJournalTrade> Trades { get; private set; } = new ObservableCollection<IJournalTrade>();

        public ObservableCollection<ISelectable> Markets { get; } = new ObservableCollection<ISelectable>();

        public ObservableCollection<ISelectableStrategy> Strategies { get; } = new ObservableCollection<ISelectableStrategy>();

        public ICommand AddNewTradeCommand => new BasicCommand(AddNewTrade);

        public ICommand EditTradeCommand => new BasicCommand(EditTrade);

        public ICommand RemoveTradeCommand => new BasicCommand(RemoveTrade);

        public ICommand PopOutGraphCommand => new BasicCommand(PopOutGraph);

        public TradeFilterSelectorViewModel FilterSelector { get; }

        public IJournalTrade SelectedTrade { get; set; }

        public double AccountStartSize
        {
            get => _accountStartSize;
            set
            {
                _accountStartSize = value;
                UpdateGraph();
                StrategyResultsStatsViewModel.UpdateAccountStartSize(_accountStartSize);
            }
        }

        public StrategyResultsStatsViewModel StrategyResultsStatsViewModel
        {
            get => _strategyResultsStatsViewModel;
            set => SetProperty(ref _strategyResultsStatsViewModel, value);
        }

        public TradeJournalViewModel(IRunner runner)
        {
            ReadInSavedTrades();
            PopulateSelectables();

            _unfilteredTrades = Trades;
            _runner = runner;

            FilterSelector = new TradeFilterSelectorViewModel(Strategies, Markets);
            FilterSelector.FiltersChanged += OnTradeFiltersChanged;

            UpdateDateFilters();
            StrategyResultsStatsViewModel = new StrategyResultsStatsViewModel(GetStrategyStats(Trades.ToList<ITrade>(), AccountStartSize), _runner);
            UpdateGraph();
        }

        private void AddNewTrade()
        {
            _addTradeViewModel = new AddTradeViewModel(_runner, Markets, Strategies);
            _addTradeViewModel.TradeAdded += AddTrade;
            _runner.GetTradeDetails(_addTradeViewModel);
        }

        private void OnTradeFiltersChanged(object sender, EventArgs e)
        {
            FilterTrades();
        }

        private void FilterTrades()
        {
            var marketFilteredTrades = RemoveUnselectedMarkets(_unfilteredTrades);
            var strategyFilteredTrades = RemoveUnselectedStrategies(marketFilteredTrades);
            var dateFilteredTrades = RemoveTradesOutsideDateRange(strategyFilteredTrades);
            var timeFilteredTrades = RemoveTradesOutsideTimeRange(dateFilteredTrades);
            var riskRewardRatioFilteredTrades = RemoveTradesOutsideRiskRewardRatioRange(timeFilteredTrades);
            var statusFilteredTrades = RemoveStatusSelectedTrades(riskRewardRatioFilteredTrades);
            var directionFilteredTrades = RemoveUnselectedTradeDirections(statusFilteredTrades);
            var dayFilteredTrades = RemoveFilteredDays(directionFilteredTrades);

            Trades = dayFilteredTrades;
            RaisePropertyChanged(nameof(Trades));
            UpdateDateFilters();
            StrategyResultsStatsViewModel = new StrategyResultsStatsViewModel(GetStrategyStats(Trades.ToList<ITrade>(), AccountStartSize), _runner);
            UpdateGraph();
        }

        private IEnumerable<IJournalTrade> RemoveUnselectedMarkets(IEnumerable<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                foreach (var market in Markets)
                {
                    if (market.IsSelected && market.Name == trade.Market.Name)
                    {
                        newList.Add(trade);
                    }
                }

            }

            return newList;
        }

        private IEnumerable<IJournalTrade> RemoveUnselectedStrategies(IEnumerable<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                if (trade.Strategy != null)
                {
                    foreach (var strategy in Strategies)
                    {
                        if (strategy.IsSelected && strategy.Name == trade.Strategy.Name)
                        {
                            newList.Add(trade);
                        }
                    }
                }
            }

            return newList;
        }

        private IEnumerable<IJournalTrade> RemoveTradesOutsideDateRange(IEnumerable<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                if (trade.OpenTime.Date >= FilterSelector.FilterStartDate.Date && trade.OpenTime.Date <= FilterSelector.FilterEndDate.Date)
                {
                    newList.Add(trade);
                }
            }

            return newList;
        }

        private IEnumerable<IJournalTrade> RemoveTradesOutsideTimeRange(IEnumerable<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                if (trade.OpenTime.TimeOfDay >= FilterSelector.FilterStartTime.TimeOfDay &&
                    trade.OpenTime.TimeOfDay <= FilterSelector.FilterEndTime.TimeOfDay)
                {
                    newList.Add(trade);
                }
            }

            return newList;
        }

        private IEnumerable<IJournalTrade> RemoveTradesOutsideRiskRewardRatioRange(IEnumerable<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                if (trade.RiskRewardRatio >= FilterSelector.MinRiskRewardRatio &&
                    trade.RiskRewardRatio <= FilterSelector.MaxRiskRewardRatio)
                {
                    newList.Add(trade);
                }
            }

            return newList;
        }

        private IEnumerable<IJournalTrade> RemoveStatusSelectedTrades(IEnumerable<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                if (FilterSelector.SelectedTradeStatus == TradeStatus.Both || FilterSelector.SelectedTradeStatus == TradeStatus.Closed)
                {
                    trade.CloseLevel.IfExistsThen(x => { newList.Add(trade); });
                }

                if (FilterSelector.SelectedTradeStatus == TradeStatus.Both || FilterSelector.SelectedTradeStatus == TradeStatus.Open)
                {
                    trade.CloseLevel.IfEmpty(() => { newList.Add(trade); });
                }
            }

            return newList;
        }

        private IEnumerable<IJournalTrade> RemoveUnselectedTradeDirections(IEnumerable<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                if (FilterSelector.SelectedTradeDirection == TradeDirection.Both || FilterSelector.SelectedTradeDirection == trade.Direction)
                {
                    newList.Add(trade);
                }
            }

            return newList;
        }

        private ObservableCollection<IJournalTrade> RemoveFilteredDays(IEnumerable<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                foreach (var day in FilterSelector.DaysOfWeek)
                {
                    if (day.IsSelected && day.Name == trade.OpenTime.DayOfWeek.ToString())
                    {
                        newList.Add(trade);
                    }
                }
            }

            return newList;
        }

        private void UpdateDateFilters()
        {
            if (_unfilteredTrades.Count < 1)
            {
                return;
            }

            var startDate = DateTime.MaxValue;
            var endDate = DateTime.MinValue;

            foreach (var trade in _unfilteredTrades)
            {
                if (trade.OpenTime.Date < startDate)
                {
                    startDate = trade.OpenTime.Date;
                }

                if (trade.OpenTime.Date > endDate)
                {
                    endDate = trade.OpenTime.Date;
                }
            }

            FilterSelector.FilterStartDate = startDate;
            FilterSelector.TradesStartDate = startDate;
            FilterSelector.FilterEndDate = endDate;
            FilterSelector.TradesEndDate = endDate;
        }

        private void AddTrade(object sender, EventArgs e)
        {
            var openTime = new DateTime(_addTradeViewModel.OpenDate.Year, _addTradeViewModel.OpenDate.Month,
                _addTradeViewModel.OpenDate.Day, _addTradeViewModel.OpenTime.Hour, _addTradeViewModel.OpenTime.Minute,
                _addTradeViewModel.OpenTime.Second);

            var closeTime = Option.None<DateTime>();

            _addTradeViewModel.CloseLevel.IfExistsThen(x =>
            {
                closeTime = Option.Some(new DateTime(_addTradeViewModel.CloseDate.Year, _addTradeViewModel.CloseDate.Month,
                    _addTradeViewModel.CloseDate.Day, _addTradeViewModel.CloseTime.Hour,
                    _addTradeViewModel.CloseTime.Minute, _addTradeViewModel.CloseTime.Second));
            });

            var trade = new JournalTrade(_addTradeViewModel.Entry, _addTradeViewModel.Stop, _addTradeViewModel.Target,
                _addTradeViewModel.OpenLevel, _addTradeViewModel.CloseLevel, openTime, closeTime,
                _addTradeViewModel.SelectedMarket, _addTradeViewModel.SelectedStrategy, _addTradeViewModel.Size,
                _addTradeViewModel.MaximumAdverseExcursion, _addTradeViewModel.MaximumFavourableExcursion);

            if (_addTradeViewModel.IsEditing)
            {
                _unfilteredTrades.Remove(SelectedTrade);
            }

            _unfilteredTrades.Add(trade);
            UpdateDateFilters();
            FilterTrades();
            SaveTradeData();
        }

        private void EditTrade()
        {
            if (SelectedTrade != null)
            {
                var trade = SelectedTrade;
                _addTradeViewModel = new AddTradeViewModel(_runner, Markets, Strategies, trade);
                _addTradeViewModel.TradeAdded += AddTrade;
                _runner.GetTradeDetails(_addTradeViewModel);
            }
        }

        private void RemoveTrade()
        {
            if (_runner.RunForResult(this,
                new Message("Confirm", "Are you sure you want to remove this trade?", Message.MessageType.Question)))
            {
                _unfilteredTrades.Remove(SelectedTrade);
                UpdateDateFilters();
                FilterTrades();
                SaveTradeData();
            }
        }

        private void SaveTradeData()
        {
            CreateDirectory($"{_savedDataPath}Trades");

            var dir = new DirectoryInfo($"{_savedDataPath}Trades");

            foreach (var file in dir.GetFiles())
            {
                file.Delete();
            }

            foreach (var trade in _unfilteredTrades)
            {
                var t = new SavedTrade(trade);
                var uniqueFileName = $@"{Guid.NewGuid()}.txt";

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream($"{_savedDataPath}Trades\\{uniqueFileName}.txt", FileMode.Create, FileAccess.Write);

                formatter.Serialize(stream, t);
                stream.Close();
            }
        }

        private void ReadInSavedTrades()
        {
            CreateDirectory($"{_savedDataPath}Trades");

            var dir = new DirectoryInfo($"{_savedDataPath}Trades");
            IFormatter formatter = new BinaryFormatter();

            foreach (var file in dir.GetFiles("*.txt"))
            {
                var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                var savedTrade = (SavedTrade)formatter.Deserialize(stream);
                var journalTrade = new JournalTrade(savedTrade);
                Trades.Add(journalTrade);
            }
        }

        private void PopulateSelectables()
        {
            foreach (var trade in Trades)
            {
                var marketExists = false;
                var strategyExists = false;

                foreach (var market in Markets)
                {
                    if (trade.Market.Name == market.Name)
                    {
                        marketExists = true;
                    }
                }

                foreach (var strategy in Strategies)
                {
                    if (trade.Strategy.Name == strategy.Name)
                    {
                        strategyExists = true;
                    }
                }

                if (!marketExists)
                {
                    Markets.Add(new Market(trade.Market.Name));
                }

                if (!strategyExists)
                {
                    Strategies.Add(new Strategy<FibonacciLevel, FibonacciLevel>(trade.Strategy.Name, trade.Strategy.ShortName));
                }
            }
        }

        private void UpdateGraph()
        {
            Plot.UpdateData(AccountStartSize, Trades);
        }

        private void PopOutGraph()
        {
            var vm = new GraphWindowViewModel(AccountStartSize, Trades);

            _runner.ShowGraphWindow(vm);
        }

        private readonly string _savedDataPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Saved Data\\";

        private readonly IRunner _runner;
        private AddTradeViewModel _addTradeViewModel;
        private readonly ObservableCollection<IJournalTrade> _unfilteredTrades;
        private StrategyResultsStatsViewModel _strategyResultsStatsViewModel;
        private double _accountStartSize = 10000;
    }
}
