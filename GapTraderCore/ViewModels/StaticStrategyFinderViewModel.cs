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

        public ICommand ViewTradesCommand => new BasicCommand(() => Runner.ShowTrades(this, SelectedStrategy));

        public ICommand ViewGraphCommand => new BasicCommand(() =>
            Runner.ShowGraphWindow(new GraphWindowViewModel(SelectedStrategy.Stats.StartBalance, SelectedStrategy.Trades)));

        public ICommand MoreDetailsCommand => new BasicCommand(() =>
            Runner?.ShowStrategyStatsWindow(new StrategyResultsStatsViewModel(SelectedStrategy.Stats, SelectedStrategy.Name)));

        public StaticStrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market, IRunner runner,
            AccountSizerViewModel accountSizer) : base(strategyTester, market, runner, accountSizer)
        {
            VariableSelector = _selector;
            _selector.FiltersChanged += RefreshStrategies;
        }

        public override void ClearSearchResults()
        {
            Strategies = new List<IGapFillStrategy>();
            base.ClearSearchResults();
        }

        public override void FindStrategies(StrategyTestFilters filters, TradeDirection tradeDirection)
        {
            LoadingBar.Maximum = GetNumberOfCombinations();

            int entryStartIndex;
            int entryEndIndex;
            int targetStartIndex;
            int targetEndIndex;

            if (_selector.IsFixedEntry)
            {
                entryStartIndex = Array.IndexOf(Enum.GetValues(typeof(FibonacciLevel)), _selector.SelectedEntry);
                entryEndIndex = Array.IndexOf(Enum.GetValues(typeof(FibonacciLevel)), _selector.SelectedEntry);
            }
            else
            {
                (entryStartIndex, entryEndIndex) = SetEntryFibIndexes();
            }

            if (_selector.IsFixedTarget)
            {
                targetStartIndex = Array.IndexOf(Enum.GetValues(typeof(FibonacciLevel)), _selector.SelectedTarget);
                targetEndIndex = Array.IndexOf(Enum.GetValues(typeof(FibonacciLevel)), _selector.SelectedTarget);
            }
            else
            {
                (targetStartIndex, targetEndIndex) = SetTargetFibIndexes();
            }

            StrategyTester.IsFibEntry = true;
            StrategyTester.IsFibTarget = true;
            StrategyTester.IsFixedStop = _selector.IsFixedStop;

            var dateTimeFilters = _selector.ApplyDateTimeFilters
                ? filters
                : new StrategyTestFilters(Market.DataDetails.StartDate, Market.DataDetails.EndDate,
                    Market.DataDetails.OpenTime, Market.DataDetails.CloseTime);

            var fibs = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));
            var tempStrategies = new List<IGapFillStrategy>(); 

            StrategyTester.SelectedDirection = tradeDirection;

            for (var t = _selector.MinStopTrail; t <= _selector.MaxStopTrail; t+= _selector.StopTrailIncrement)
            {
                for (var m = _selector.MinMinGapSize; m <= _selector.MaxMinGapSize; m += _selector.GapSizeIncrement)
                {
                    for (var k = _selector.MinStopSize; k <= _selector.MaxStopSize; k += _selector.StopSizeIncrement)
                    {
                        for (var i = entryStartIndex; i <= entryEndIndex; i++)
                        {
                            for (var j = targetStartIndex; j <= targetEndIndex; j++)
                            {
                                StrategyTester.FibLevelEntry = fibs[i];
                                StrategyTester.FibLevelTarget = fibs[j];
                                StrategyTester.Stop = k;
                                StrategyTester.IsStopTrailed = _selector.IsStopTrailed;
                                StrategyTester.TrailedStopSize = t;
                                StrategyTester.SetSizing(AccountSizer.AccountStartSize, AccountSizer.RiskPercentage, AccountSizer.Compound);
                                StrategyTester.TestStrategy(Market, dateTimeFilters, m);

                                if (StrategyTester.Strategy.Stats.CashProfit > 0)
                                {
                                    tempStrategies.Add(StrategyTester.Strategy);
                                }

                                LoadingBar.Progress++;
                            }
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

        public override void UpdateTester(GapFillStrategyTester tester)
        {
            var strategyType = StrategyType.OutOfGap;

            if (tester is IntoGapStrategyTester)
            {
                strategyType = StrategyType.IntoGap;
            }

            _selector.UpdateFixedFibs(strategyType);
            base.UpdateTester(tester);
        }

        protected override void StartStrategySearch()
        {
            Strategies = new List<IGapFillStrategy>();
            base.StartStrategySearch();
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

            var stopSizeMultiplier = (double)(_selector.MaxStopSize - _selector.MinStopSize) /
                                 _selector.StopSizeIncrement;

            if (Math.Abs(stopSizeMultiplier % 1) < 0.0001)
            {
                stopSizeMultiplier++;
            }
            else
            {
                stopSizeMultiplier = Math.Ceiling(stopSizeMultiplier);
            }

            var stopTrailMultiplier = 1.0;

            if (_selector.IsStopTrailed)
            {
                stopTrailMultiplier = (double) (_selector.MaxStopTrail - _selector.MinStopTrail) / _selector.StopTrailIncrement;

                if (Math.Abs(stopTrailMultiplier % 1) < 0.0001)
                {
                    stopTrailMultiplier++;
                }
                else
                {
                    stopTrailMultiplier = Math.Ceiling(stopTrailMultiplier);
                }
            }

            var entryMultiplier = _selector.IsFixedEntry ? 1 : 10;
            var targetMultiplier = _selector.IsFixedTarget ? 1 : 9;

            return (int)gapMultiplier * stopSizeMultiplier * stopTrailMultiplier * entryMultiplier * targetMultiplier;
        }

        private readonly StaticStrategyVariableSelector _selector = new StaticStrategyVariableSelector();

        private List<IGapFillStrategy> _strategies = new List<IGapFillStrategy>();
        private List<IGapFillStrategy> _unFilteredStrategies = new List<IGapFillStrategy>();
    }
}
