using System;
using System.Collections.Generic;
using System.Windows.Input;
using Foundations;
using GapTraderCore.Interfaces;
using GapTraderCore.StrategyTesters;
using GapTraderCore.VariableSelectors;

namespace GapTraderCore.ViewModels
{
    public sealed class StaticStrategyFinderViewModel : StrategyFinderViewModel
    {
        public List<IGapFillStrategy> Strategies
        {
            get => _strategies;
            private set => SetProperty(ref _strategies, value);
        }

        public ICommand ViewTradesCommand => new BasicCommand(ViewTrades);

        public StaticStrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market, IRunner runner,
            AccountSizerViewModel accountSizer) : base(strategyTester, market, runner, accountSizer)
        {
            VariableSelector = _selector;
            _selector.FiltersChanged += RefreshStrategies;
        }

        public override void FindStrategies(StrategyTestFilters filters, TradeDirection tradeDirection)
        {
            LoadingBar.Maximum = GetNumberOfCombinations();

            var (entryStartIndex, entryEndIndex, targetStartIndex, targetEndIndex) = SetFibIndexes();

            StrategyTester.IsFibEntry = true;
            StrategyTester.IsFibTarget = true;
            StrategyTester.IsFixedStop = _selector.IsFixedStop;

            var dateTimeFilters = _selector.ApplyDateTimeFilters
                ? filters
                : new StrategyTestFilters(Market.DataDetails.StartDate, Market.DataDetails.EndDate,
                    Market.DataDetails.OpenTime, Market.DataDetails.CloseTime);

            var fibs = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));
            var tempStrategies = new List<IGapFillStrategy>();

            StrategyTester.SetSizing(AccountSizer.AccountStartSize, AccountSizer.RiskPercentage, AccountSizer.Compound);

            StrategyTester.SelectedDirection = tradeDirection;

            for (var m = _selector.MinMinGapSize; m <= _selector.MaxMinGapSize; m += _selector.GapSizeIncrement)
            {
                for (var k = _selector.MinStop; k <= _selector.MaxStop; k += _selector.StopIncrement)
                {
                    for (var i = entryStartIndex; i <= entryEndIndex; i++)
                    {
                        for (var j = targetStartIndex; j <= targetEndIndex; j++)
                        {
                            StrategyTester.FibLevelEntry = fibs[i];
                            StrategyTester.FibLevelTarget = fibs[j];
                            StrategyTester.Stop = k;
                            StrategyTester.TestStrategy(Market, dateTimeFilters, m);

                            if (StrategyTester.Strategy.Stats.PointsTotal > 0)
                            {
                                tempStrategies.Add(StrategyTester.Strategy);
                            }

                            LoadingBar.Progress++;
                        }
                    }
                }
            }

            tempStrategies.Sort((x, y) => x.Stats.CashProfit.CompareTo(y.Stats.CashProfit));
            tempStrategies.Reverse();
            _unFilteredStrategies = tempStrategies;
            Strategies = FilterStrategies(tempStrategies);
            
            VariableSelector = _selector;
            FinishSearch();
        }

        protected override void StartStrategySearch()
        {
            Strategies = new List<IGapFillStrategy>();
            base.StartStrategySearch();
        }

        private void ViewTrades()
        {
            Runner.ShowTrades(this, SelectedStrategy);
        }

        private void RefreshStrategies(object sender, EventArgs e)
        {
            Strategies = FilterStrategies(_unFilteredStrategies);
        }

        private List<IGapFillStrategy> FilterStrategies(IEnumerable<IGapFillStrategy> strategies)
        {
            var newList = new List<IGapFillStrategy>();

            foreach (var strategy in strategies)
            {
                if (strategy.Stats.WinProbability >= _selector.MinWinProbability / 100 &&
                    strategy.Stats.ProfitFactor >= _selector.MinProfitFactor &&
                    strategy.Stats.TradeCount >= _selector.MinTrades)
                {
                    newList.Add(strategy);
                }
            }

            return newList;
        }

        private double GetNumberOfCombinations()
        {
            var gapMultiplier = (double)(_selector.MaxMinGapSize - _selector.MinMinGapSize) /
                                _selector.GapSizeIncrement;

            if (Math.Abs(gapMultiplier % 1) < 0.0001)
            {
                gapMultiplier++;
            }
            else
            {
                gapMultiplier = Math.Ceiling(gapMultiplier);
            }

            var stopMultiplier = (double)(_selector.MaxStop - _selector.MinStop) /
                                 _selector.StopIncrement;

            if (Math.Abs(stopMultiplier % 1) < 0.0001)
            {
                stopMultiplier++;
            }
            else
            {
                stopMultiplier = Math.Ceiling(stopMultiplier);
            }

            return (int)gapMultiplier * stopMultiplier * 90;
        }

        private readonly StaticStrategyVariableSelector _selector = new StaticStrategyVariableSelector();

        private List<IGapFillStrategy> _strategies = new List<IGapFillStrategy>();
        private List<IGapFillStrategy> _unFilteredStrategies = new List<IGapFillStrategy>();
    }
}
