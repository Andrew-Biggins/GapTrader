using Foundations;
using GapAnalyser.Candles;
using GapAnalyser.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using static GapAnalyser.TradeServices;

namespace GapAnalyser.StrategyTesters
{
    public abstract class StrategyTester : BindableBase
    {
        public List<ITrade> Trades { get; set; } = new List<ITrade>();

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

        protected virtual void FinalizeStats()
        {
            CheckLoseStreak();
            CheckWinStreak();
            var averagePointsWin = _pointsWinTotal / _wins;
            var averagePointsLoss = _pointsLossTotal / _loses;
            var averageCashWin = _cashWinTotal / _wins;
            var averageCashLoss = _cashLossTotal / _loses;

            var profitFactor = _pointsWinTotal / -_pointsLossTotal; // todo is this wrong, and expectancy?

            Stats = new StrategyStats(_wins, _loses, _longestWinningStreak, _longestLosingStreak, _pointsTotal,
                _cashProfit, _biggestPointsWin, _biggestCashWin, averagePointsWin, averagePointsLoss, averageCashWin,
                averageCashLoss, profitFactor);

            ResetValues();
        }

        protected void CompareMinuteData(DailyCandle candle, Dictionary<DateTime, List<MinuteCandle>> minuteData,
            TimeSpan startTime, TimeSpan endTime)
        {
            if (minuteData.TryGetValue(candle.Date, out var minuteCandles))
            {
                var (entry, stop, target, stopSize) = CalculateTradeLevels(candle);

                var trade = AttemptTrade(minuteCandles, entry, stop, target, startTime, endTime);

                var risk = _compound 
                    ? _balance * _riskPercentage / 100 
                    : _startBalance * _riskPercentage / 100;

                trade.IfExistsThen(t =>
                {
                    t.AddProfit(risk/stopSize);
                    UpdateStatistics(t.PointsProfit, t.CashProfit);
                    Trades.Add(t);
                });
            }
        }

        protected abstract (double, double, double, double) CalculateTradeLevels(DailyCandle candle);

        private void ResetValues()
        {
            _pointsTotal = 0;
            _cashProfit = 0;
            _biggestPointsWin = 0;
            _biggestCashWin = 0;
            _longestLosingStreak = 0;
            _longestWinningStreak = 0;
            _loseStreak = 0;
            _winStreak = 0;
            _wins = 0;
            _loses = 0;
            _pointsWinTotal = 0;
            _pointsLossTotal = 0;
            _cashWinTotal = 0;
            _cashLossTotal = 0;
            _balance = _startBalance;
        }

        private void UpdateStatistics(double pointsResult, double cashProfit)
        {
            _pointsTotal += pointsResult;
            _cashProfit += cashProfit;
            _balance += cashProfit;

            if (pointsResult > _biggestPointsWin)
            {
                _biggestPointsWin = pointsResult;
            }

            if (cashProfit > _biggestCashWin)
            {
                _biggestCashWin = cashProfit;
            }

            if (pointsResult > 0)
            {
                if (!_previousTradeWon)
                {
                    CheckLoseStreak();
                    _loseStreak = 0;
                }

                _wins++;
                _pointsWinTotal += pointsResult;
                _cashWinTotal += cashProfit;
                _winStreak++;
                _previousTradeWon = true;
            }
            else if (pointsResult < 0)
            {
                if (_previousTradeWon)
                {
                    CheckWinStreak();
                    _winStreak = 0;
                }

                _loses++;
                _pointsLossTotal += pointsResult;
                _cashLossTotal += cashProfit;
                _loseStreak++;
                _previousTradeWon = false;
            }
        }

        private void CheckWinStreak()
        {
            if (_winStreak > _longestWinningStreak)
            {
                _longestWinningStreak = _winStreak;
            }
        }

        private void CheckLoseStreak()
        {
            if (_loseStreak > _longestLosingStreak)
            {
                _longestLosingStreak = _loseStreak;
            }
        }

        protected StrategyStats Stats;
        protected ITradeLevelCalculator TradeLevelCalculator;

        private double _pointsTotal;
        private double _cashProfit;
        private int _wins;
        private int _loses;
        private int _longestWinningStreak;
        private int _longestLosingStreak;
        private double _biggestPointsWin;
        private double _biggestCashWin;
        private double _pointsWinTotal;
        private double _cashWinTotal;
        private double _pointsLossTotal;
        private double _cashLossTotal;
        private int _winStreak;
        private int _loseStreak;
        private bool _previousTradeWon;
        private double _balance;
        private double _startBalance;
        private double _riskPercentage;
        private bool _compound;
    }
}








