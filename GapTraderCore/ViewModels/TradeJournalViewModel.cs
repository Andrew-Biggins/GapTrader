using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Foundations;
using Foundations.Optional;
using GapTraderCore.Interfaces;
using GapTraderCore.Strategies;
using GapTraderCore.Trades;
using static GapTraderCore.DataProcessor;

namespace GapTraderCore.ViewModels
{
    public sealed class TradeJournalViewModel : BindableBase
    {
        public ObservableCollection<IJournalTrade> Trades { get; set; } = new ObservableCollection<IJournalTrade>();

        public ObservableCollection<IMarket> Markets { get; set; }

        public ObservableCollection<IStrategy> Strategies { get; set; }

        public ICommand AddNewTradeCommand => new BasicCommand(() => _runner.GetTradeDetails(_addTradeViewModel));

        public TradeFilterSelectorViewModel FilterSelector { get; }

        public StrategyResultsStatsViewModel StrategyResultsStatsViewModel
        {
            get => _strategyResultsStatsViewModel;
            set => SetProperty(ref _strategyResultsStatsViewModel, value);
        }

        public TradeJournalViewModel(IRunner runner)
        {
            Markets = new ObservableCollection<IMarket>();
            Markets.Add(new Market { Name = "DJI" });
            Markets.Add(new Market { Name = "FTSE" });
            Markets.Add(new Market { Name = "DAX" });
            Markets.Add(new Market { Name = "EURUSD" });
            Markets.Add(new Market { Name = "USDJPY" });
            Markets.Add(new Market { Name = "GBPUSD" });
            Markets.Add(new Market { Name = "NZDUSD" });
            Markets.Add(new Market { Name = "AUDUSD" });
            Markets.Add(new Market { Name = "GBPJPY" });
            Markets.Add(new Market { Name = "GBPNZD" });


            Strategies = new ObservableCollection<IStrategy>();
            var stats = new StrategyStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            var trades = new List<ITrade>();
            var title = "Entry:5.9% | Target: 200% | Stop: 60pts";
            var strategy = new Strategy<FibonacciLevel, FibonacciLevel>(FibonacciLevel.FivePointNine, 60, FibonacciLevel.TwoHundred, stats, trades, title);
            Strategies.Add(strategy);

            Trades.Add(new JournalTrade(6000, 5900, 6200, 6000, Option.None<double>(), new DateTime(2020, 6, 12, 8, 30, 0), Option.None<DateTime>(), Markets[0], Strategies[0], 5));
            _unfilteredTrades = Trades;
            _runner = runner;
            _addTradeViewModel = new AddTradeViewModel(_runner, Markets, Strategies);
            _addTradeViewModel.TradeAdded += AddTrade;

            FilterSelector = new TradeFilterSelectorViewModel(Strategies, Markets);
            FilterSelector.FiltersChanged += OnTradeFiltersChanged;

            UpdateDateFilters();
        }

        private void OnTradeFiltersChanged(object sender, EventArgs e)
        {
            FilterTrades();
        }

        private void FilterTrades()
        {
            var x = RemoveUnselectedMarkets(_unfilteredTrades);
            var y = RemoveUnselectedStrategies(x);
            var z = RemoveTradesOutsideDateRange(y);
            var q = RemoveTradesOutsideTimeRange(z);
            var p = RemoveTradesOutsideRiskRewardRatioRange(q);
            var r = RemoveStatusSelectedTrades(p);

            Trades = r;
            RaisePropertyChanged(nameof(Trades));
            UpdateDateFilters();
            StrategyResultsStatsViewModel = new StrategyResultsStatsViewModel(GetStrategyStats(Trades));
        }

        private ObservableCollection<IJournalTrade> RemoveUnselectedMarkets(ObservableCollection<IJournalTrade> trades)
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

        private ObservableCollection<IJournalTrade> RemoveUnselectedStrategies(ObservableCollection<IJournalTrade> trades)
        {
            var newList = new ObservableCollection<IJournalTrade>();

            foreach (var trade in trades)
            {
                foreach (var strategy in Strategies)
                {
                    if (strategy.IsSelected && strategy.Title == trade.Strategy.Title)
                    {
                        newList.Add(trade);
                    }
                }
            }

            return newList;
        }

        private ObservableCollection<IJournalTrade> RemoveTradesOutsideDateRange(ObservableCollection<IJournalTrade> trades)
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

        private ObservableCollection<IJournalTrade> RemoveTradesOutsideTimeRange(ObservableCollection<IJournalTrade> trades)
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

        private ObservableCollection<IJournalTrade> RemoveTradesOutsideRiskRewardRatioRange(ObservableCollection<IJournalTrade> trades)
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

        private ObservableCollection<IJournalTrade> RemoveStatusSelectedTrades(ObservableCollection<IJournalTrade> trades)
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
                _addTradeViewModel.SelectedMarket, _addTradeViewModel.SelectedStrategy, _addTradeViewModel.Size);

            _unfilteredTrades.Add(trade);
            UpdateDateFilters();
            FilterTrades();
        }

        private readonly IRunner _runner;
        private readonly AddTradeViewModel _addTradeViewModel;
        private readonly ObservableCollection<IJournalTrade> _unfilteredTrades = new ObservableCollection<IJournalTrade>();
        private StrategyResultsStatsViewModel _strategyResultsStatsViewModel = new StrategyResultsStatsViewModel();
    }
}
