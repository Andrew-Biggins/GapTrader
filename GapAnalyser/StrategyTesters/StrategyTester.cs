using System;
using System.Collections.Generic;
using System.Diagnostics;
using Foundations;
using GapAnalyser.Candles;
using GapAnalyser.Interfaces;
using static GapAnalyser.TradeServices;

namespace GapAnalyser.StrategyTesters
{
    public abstract class StrategyTester : BindableBase
    {
        protected StrategyTester(ITradeLevelCalculator tradeLevelCalculator)
        {
            TradeLevelCalculator = tradeLevelCalculator;
        }

        protected virtual void FinalizeStats()
        {
            CheckLoseStreak();
            CheckWinStreak();
            var averageWin = _winTotal / _wins;
            var averageLoss = _lossTotal / _loses;
            var profitFactor = _winTotal / -_lossTotal;

            Stats = new StrategyStats(_wins, _loses, _longestWinningStreak, _longestLosingStreak, _profit, _biggestWin,
                averageWin, averageLoss, profitFactor);

            ResetValues();
        }

        protected void CompareMinuteData(DailyCandle candle, Dictionary<DateTime, List<MinuteCandle>> minuteData, TimeSpan startTime, TimeSpan endTime)
        {
            if (minuteData.TryGetValue(candle.Date, out var minuteCandles))
            {
                var (entry, stop, target) = CalculateTradeLevels(candle);

                var trade = AttemptTrade(minuteCandles, entry, stop, target, startTime, endTime);

                trade.IfExistsThen(t =>
                {
                    UpdateStatistics(t.Profit);
                    Debug.WriteLine(candle.Gap.GapPoints);
                    Debug.WriteLine(
                        $"{candle.Date.Date} {t.OpenLevel} {t.OpenTime.TimeOfDay} {t.CloseLevel} {t.CloseTime.TimeOfDay} {t.Profit}");
                });
            }
        }

        protected abstract (double, double, double) CalculateTradeLevels(DailyCandle candle);

        private void ResetValues()
        {
            _profit = 0;
            _biggestWin = 0;
            _longestLosingStreak = 0;
            _longestWinningStreak = 0;
            _loseStreak = 0;
            _winStreak = 0;
            _wins = 0;
            _loses = 0;
            _winTotal = 0;
            _lossTotal = 0;
        }

        private void UpdateStatistics(double result)
        {
            _profit += result;
            //_tradeCount++; 

            if (result > _biggestWin)
            {
                _biggestWin = result;
            }

            if (result > 0)
            {
                if (!_previousTradeWon)
                {
                    CheckLoseStreak();
                }

                _wins++;
                _winTotal += result;
                _winStreak++;
                _previousTradeWon = true;
            }
            else if (result < 0)
            {
                if (_previousTradeWon)
                {
                    CheckWinStreak();
                }

                _loses++;
                _lossTotal += result;
                _loseStreak++;
                _previousTradeWon = false;
            }
        }

        private void CheckWinStreak()
        {
            if (_winStreak > _longestWinningStreak)
            {
                _longestWinningStreak = _winStreak;
                _winStreak = 0;
            }
        }

        private void CheckLoseStreak()
        {
            if (_loseStreak > _longestLosingStreak)
            {
                _longestLosingStreak = _loseStreak;
                _loseStreak = 0;
            }
        }

        protected StrategyStats Stats;
        protected ITradeLevelCalculator TradeLevelCalculator;

        private double _profit;
        private int _wins;
        private int _loses;
        private int _longestWinningStreak;
        private int _longestLosingStreak;
        private double _biggestWin;
        private double _winTotal;
        private double _lossTotal;
        private int _winStreak;
        private int _loseStreak;
        private bool _previousTradeWon;
       // private int _tradeCount;
    }
}








