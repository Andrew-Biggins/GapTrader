using System;
using System.Collections.Generic;
using Foundations;
using GapTraderCore.Candles;
using GapTraderCore.Interfaces;
using static GapTraderCore.TradeServices;
using static GapTraderCore.DataProcessor;

namespace GapTraderCore.StrategyTesters
{
    public abstract class StrategyTester : BindableBase
    {
        public List<ITrade> Trades { get; set; } = new List<ITrade>();

        public TradeDirection SelectedDirection { get; set; } = TradeDirection.Both;

        public bool IsFixedStop
        {
            get => _isFixedStop;
            set => SetProperty(ref _isFixedStop, value);
        }

        public void SetSizing(double startBalance, double riskPercentage, bool compound)
        {
            _startBalance = startBalance;
            _balance = startBalance;
            _riskPercentage = riskPercentage;
            _compound = compound;
        }

        protected StrategyTester(ITradeLevelCalculator tradeLevelCalculator)
        {
            TradeLevelCalculator = tradeLevelCalculator;
        }

        protected void CompareMinuteData(DailyCandle candle, Dictionary<DateTime, List<BidAskCandle>> minuteData,
            TimeSpan startTime, TimeSpan endTime)
        {
            if (minuteData.TryGetValue(candle.Date.Date, out var minuteCandles))
            {
                var (entry, stop, target) = CalculateTradeLevels(candle);

                var direction = target > stop ? TradeDirection.Long : TradeDirection.Short;

                if (SelectedDirection == direction || SelectedDirection == TradeDirection.Both)
                {
                    var risk = _compound
                        ? _balance * _riskPercentage / 100
                        : _startBalance * _riskPercentage / 100;

                    var trade = AttemptTrade(minuteCandles, entry, stop, target, startTime, endTime, risk);

                    trade.IfExistsThen(t =>
                    {
                        t.PointsProfit.IfExistsThen(x =>
                        {
                            _balance += t.CashProfit;
                        });

                        Trades.Add(t);
                    });
                }
            }

            Stats = GetStrategyStats(Trades);
        }

        protected abstract (double, double, double) CalculateTradeLevels(DailyCandle candle);
        
        protected StrategyStats Stats;
        protected ITradeLevelCalculator TradeLevelCalculator;

        private double _balance;
        private double _startBalance;
        private double _riskPercentage;
        private bool _compound;
        private bool _isFixedStop;
    }
}








