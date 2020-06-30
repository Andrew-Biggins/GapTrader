using System;
using System.Collections.Generic;
using System.Globalization;
using GapTraderCore.Interfaces;
using GapTraderCore.StrategyTesters;
using GapTraderCore.TradeCalculators;
using GapTraderCore.VariableSelectors;

namespace GapTraderCore.ViewModels
{
    public enum TradeType
    {
        IntoGap,
        OutOfGap,
        Both
    }

    public sealed class DynamicStrategyFinderViewModel : StrategyFinderViewModel
    {
        public List<ITrade> Trades
        {
            get => _trades;
            private set => SetProperty(ref _trades, value);
        }

        public TradeType TradeType
        {
            get => _tradeType;
            set => SetProperty(ref _tradeType, value);
        }

        public double Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }

        public string EndBalance
        {
            get => _endBalance;
            set => SetProperty(ref _endBalance, value);
        }

        public DynamicStrategyFinderViewModel(GapFillStrategyTester strategyTester, IMarket market, IRunner runner,
            AccountSizerViewModel accountSizer) : base(strategyTester, market, runner, accountSizer)
        {
            _selector = new DynamicTestVariableSelector(market.DataDetails);
            VariableSelector = _selector;
        }

        protected override void StartStrategySearch()
        {
            Trades = new List<ITrade>();
            base.StartStrategySearch();
        }

        public override void FindStrategies(StrategyTestFilters filters, TradeDirection tradeDirection)
        {
            LoadingBar.Maximum = (Market.DataDetails.EndDate - _selector.TestStartDate).Days;
            Balance = AccountSizer.AccountStartSize;
            _tradeDirection = tradeDirection;

            _tempTrades = new List<ITrade>();

            var currentDate = _selector.TestStartDate;

            // Default to -1 to detect when daily candle is not found
            double gapSize = -1;

            while (currentDate <= Market.DataDetails.EndDate)
            {
                foreach (var candle in Market.DailyCandles)
                {
                    if (candle.Date == currentDate)
                    {
                        gapSize = candle.Gap.AbsoluteGapPoints;
                        break;
                    }
                }

                if (Math.Abs(gapSize - (-1)) < 0)
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }

                if (TradeType == TradeType.Both || TradeType == TradeType.OutOfGap)
                {
                    StrategyTester = new OutOfGapStrategyTester(new GapTradeLevelCalculator());
                    StrategyTester.SelectedDirection = _tradeDirection; // todo refactor to share with below?
                    StrategyTester.IsFixedStop = _selector.IsFixedStop;
                    Search(gapSize, currentDate);
                }

                if (TradeType == TradeType.Both || TradeType == TradeType.IntoGap)
                {
                    StrategyTester = new IntoGapStrategyTester(new GapTradeLevelCalculator());
                    StrategyTester.SelectedDirection = _tradeDirection;
                    StrategyTester.IsFixedStop = _selector.IsFixedStop;
                    Search(gapSize, currentDate);
                }

                LoadingBar.Progress++;
                currentDate = currentDate.AddDays(1);
            }

            Trades = _tempTrades;

            EndBalance = Math.Round(Balance, 2).ToString(CultureInfo.CurrentCulture);
            VariableSelector = _selector;
            FinishSearch();
        }

        private void Search(double gapSize, DateTime currentDate)
        {
            var strategyFindEndDate = currentDate.AddDays(-1);
            var strategies = GetStrategies(gapSize, strategyFindEndDate);

            if (strategies.Count != 0)
            {
                strategies.Sort((x, y) => x.Stats.CashProfit.CompareTo(y.Stats.CashProfit));
                strategies.Reverse();

                foreach (var strategy in strategies)
                {
                    if (strategy.Stats.WinProbability >= _selector.MinWinProbability / 100)
                    {
                        AttemptStrategy(strategy, currentDate);
                        break;
                    }
                }
            }
        }

        private List<IGapFillStrategy> GetStrategies(double gapSize, DateTime dataEndDate)
        {
            var dateTimeFilters = new StrategyTestFilters(Market.DataDetails.StartDate, dataEndDate,
                Market.DataDetails.OpenTime, Market.DataDetails.CloseTime);

            var fibs = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));
            var strategies = new List<IGapFillStrategy>();

            StrategyTester.SetSizing(AccountSizer.AccountStartSize, AccountSizer.RiskPercentage,
                AccountSizer.Compound);

            var (entryStartIndex, entryEndIndex) = SetEntryFibIndexes();
            var (targetStartIndex, targetEndIndex) = SetTargetFibIndexes();

            for (var k = _selector.MinStop; k <= _selector.MaxStop; k += _selector.StopIncrement)
            {
                for (var i = entryStartIndex; i <= entryEndIndex; i++)
                {
                    for (var j = targetStartIndex; j <= targetEndIndex; j++)
                    {
                        StrategyTester.FibLevelEntry = fibs[i];
                        StrategyTester.FibLevelTarget = fibs[j];
                        StrategyTester.Stop = k;
                        StrategyTester.TestStrategy(Market, dateTimeFilters, gapSize - _selector.GapSizeTolerance,
                            gapSize + _selector.GapSizeTolerance);

                        if (StrategyTester.Strategy.Stats.PointsTotal > 0)
                        {
                            strategies.Add(StrategyTester.Strategy);
                        }
                    }
                }
            }

            return strategies;
        }

        private void AttemptStrategy(IGapFillStrategy strategy, DateTime currentDate)
        {
            GapFillStrategyTester dynamicTester;
            switch (StrategyTester)
            {
                case OutOfGapStrategyTester _:
                    dynamicTester = new OutOfGapStrategyTester(new GapTradeLevelCalculator());
                    break;
                case IntoGapStrategyTester _:
                    dynamicTester = new IntoGapStrategyTester(new GapTradeLevelCalculator());
                    break;
                default:
                    return;
            }

            dynamicTester.SetSizing(Balance, AccountSizer.RiskPercentage, AccountSizer.Compound);
            dynamicTester.SelectedDirection = _tradeDirection;
            dynamicTester.IsFixedStop = _selector.IsFixedStop;
            dynamicTester.FibLevelEntry = (FibonacciLevel)strategy.Entry;
            dynamicTester.FibLevelTarget = (FibonacciLevel)strategy.Target;
            dynamicTester.Stop = strategy.Stop;

            dynamicTester.TestStrategy(Market,
                new StrategyTestFilters(currentDate, currentDate, Market.DataDetails.OpenTime,
                    Market.DataDetails.CloseTime), 0);

            if (dynamicTester.Strategy.Trades.Count > 0)
            {
                dynamicTester.Strategy.Trades[0].OpenFibLevel = (FibonacciLevel)strategy.Entry;
                dynamicTester.Strategy.Trades[0].TargetFibLevel = (FibonacciLevel)strategy.Target;
            //    dynamicTester.Strategy.Trades[0].WinProbability = strategy.Stats.WinProbability;
                _tempTrades.Add(dynamicTester.Strategy.Trades[0]);
                Balance += dynamicTester.Strategy.Trades[0].CashProfit;
            }
        }

        private readonly DynamicTestVariableSelector _selector;

        private List<ITrade> _tempTrades;
        private List<ITrade> _trades = new List<ITrade>();
        private TradeType _tradeType = TradeType.Both;
        private double _balance;
        private string _endBalance = "---";
        private TradeDirection _tradeDirection;
    }
}
