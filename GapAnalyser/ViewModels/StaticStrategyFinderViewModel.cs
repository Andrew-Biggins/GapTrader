using Foundations;
using GapAnalyser.Interfaces;
using GapAnalyser.StrategyTesters;
using GapAnalyser.VariableSelectors;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GapAnalyser.ViewModels
{
    public sealed class StaticStrategyFinderViewModel : StrategyFinderViewModel
    {
        public List<IGapFillStrategy> Strategies
        {
            get => _strategies;
            private set => SetProperty(ref _strategies, value);
        }

        public ICommand ViewTradesCommand => new BasicCommand(ViewTrades);

        public StaticStrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market, IRunner runner) :
            base(strategyTester, market, runner)
        {
            VariableSelector = _selector;
        }

        public override void FindStrategies(StrategyTestFilters filters)
        {
            LoadingBar.Maximum = GetNumberOfCombinations();

            var (entryStartIndex, entryEndIndex, targetStartIndex, targetEndIndex) = SetFibIndexes();

            StrategyTester.FibEntry = true;
            StrategyTester.FibTarget = true;

            var dateTimeFilters = _selector.ApplyDateTimeFilters
                ? filters
                : new StrategyTestFilters(Market.DataDetails.StartDate, Market.DataDetails.EndDate,
                    Market.DataDetails.OpenTime, Market.DataDetails.CloseTime);

            var fibs = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));
            var tempStrategies = new List<IGapFillStrategy>();

            StrategyTester.SetSizing(_selector.AccountSizer.AccountStartSize, _selector.AccountSizer.RiskPercentage,
                _selector.AccountSizer.Compound);

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
            Strategies = tempStrategies;
            
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
    }
}
