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
        public int MaxStop { get; set; } = 1000;
        public int MinStop { get; set; } = 10;
        public int StopIncrement { get; set; } = 10;

        public List<IStrategy> Strategies { get; private set; } = new List<IStrategy>();

        public GapFillStrategyTester StrategyTester { get; set; }

        public ICommand FindStrategiesCommand => new BasicCommand(FindStrategies);

        public StrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market)
        {
            StrategyTester = strategyTester;
            _market = market;
        }

        private void FindStrategies()
        {
            StrategyTester.FibEntry = true;
            StrategyTester.FibTarget = true;
            StrategyTester.FibLevelTarget = FibonacciLevel.OneHundredAndTwentySevenPointOne;

            var filters = new StrategyTestFilters(_market.DataDetails.StartDate, _market.DataDetails.EndDate, new TimeSpan(14,30,0),new TimeSpan(21,00,0) );

            var fibs = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            var list = new List<IStrategy>();

            for (var k = MinStop; k <= MaxStop; k += StopIncrement)
            {
                for (var i = 0; i < 10; i++)
                {
                    for (var j = 10; j < 19; j++)
                    {
                        StrategyTester.FibLevelEntry = fibs[i];
                        StrategyTester.FibLevelTarget = fibs[j];
                        StrategyTester.Stop = k;
                        StrategyTester.TestStrategy(_market, filters, 200);
                        list.Add(StrategyTester.Strategy);
                    }

                }
            }
            

            Strategies = list;
            RaisePropertyChanged(nameof(Strategies));

            StrategyTester.FibLevelEntry = FibonacciLevel.FivePointNine;
            RaisePropertyChanged(nameof(StrategyTester.FibLevelEntry));
            StrategyTester.FibLevelTarget = FibonacciLevel.OneHundredAndTwentySevenPointOne;
            RaisePropertyChanged(nameof(StrategyTester.FibLevelTarget));
            StrategyTester.Stop = 100;
            RaisePropertyChanged(nameof(StrategyTester.Stop));
        }

        private readonly IMarket _market;
    }
}
