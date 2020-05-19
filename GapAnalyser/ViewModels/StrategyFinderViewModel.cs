using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Foundations;
using GapAnalyser.Interfaces;
using GapAnalyser.StrategyTesters;

namespace GapAnalyser.ViewModels
{
    public sealed class StrategyFinderViewModel : BindableBase
    {
        public EventHandler? StrategySearchEventHandler;

        public int MaxStop { get; set; } = 1000;
        public int MinStop { get; set; } = 10;
        public int StopIncrement { get; set; } = 100;

        public int MaxMinGapSize { get; set; } = 1000;
        public int MinMinGapSize { get; set; } = 10;
        public int GapSizeIncrement { get; set; } = 100;

        public List<IGapFillStrategy> Strategies { get; private set; } = new List<IGapFillStrategy>();

        public GapFillStrategyTester StrategyTester { get; set; }

        public bool ApplyDateTimeFilters { get; set; }

        public ICommand FindStrategiesCommand => new BasicCommand(StartStrategySearch);

        public StrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market)
        {
            StrategyTester = strategyTester;
            _market = market;
        }

        private void StartStrategySearch()
        {
            StrategySearchEventHandler.Raise(this);
        }

        public void FindStrategies(StrategyTestFilters filters)
        {
            const int firstFibIndex = 0;
            const int firstFibExtensionIndex = 10;
            const int lastFibIndex = 18;

            var entryStartIndex = firstFibIndex;
            var entryEndIndex = firstFibExtensionIndex - 1;
            var targetStartIndex = firstFibExtensionIndex;
            var targetEndIndex = lastFibIndex;

            if (StrategyTester is IntoGapStrategyTester)
            {
                entryStartIndex = firstFibExtensionIndex;
                entryEndIndex = lastFibIndex;
                targetStartIndex = firstFibIndex;
                targetEndIndex = firstFibExtensionIndex - 1;
            }

            StrategyTester.FibEntry = true;
            StrategyTester.FibTarget = true;

            var dateTimeFilters = ApplyDateTimeFilters 
                ? filters 
                : new StrategyTestFilters(_market.DataDetails.StartDate, _market.DataDetails.EndDate,
                    _market.DataDetails.OpenTime, _market.DataDetails.CloseTime);

            var fibs = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            var list = new List<IGapFillStrategy>();

            for (var m = MinMinGapSize; m <= MaxMinGapSize; m += GapSizeIncrement)
            {
                for (var k = MinStop; k <= MaxStop; k += StopIncrement)
                {
                    for (var i = entryStartIndex; i <= entryEndIndex; i++)
                    {
                        for (var j = targetStartIndex; j <= targetEndIndex; j++)
                        {
                            StrategyTester.FibLevelEntry = fibs[i];
                            StrategyTester.FibLevelTarget = fibs[j];
                            StrategyTester.Stop = k;
                            StrategyTester.TestStrategy(_market, dateTimeFilters, m);
                            list.Add(StrategyTester.Strategy);
                        }
                    }
                }
            }
            
            
            list.Sort((x, y) => x.Stats.Profit.CompareTo(y.Stats.Profit));
            list.Reverse();
            Strategies = list;
            RaisePropertyChanged(nameof(Strategies));

            StrategyTester.ResetLevels();
        }

        private readonly IMarket _market;
    }
}
