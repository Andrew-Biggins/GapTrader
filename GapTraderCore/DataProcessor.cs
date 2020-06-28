using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using Foundations.Optional;
using GapTraderCore.Candles;
using GapTraderCore.Interfaces;

namespace GapTraderCore
{
    public static class DataProcessor
    {
        public static Optional<(double, double, double)> CompareMinuteData(IEnumerable<BidAskCandle> candles, DailyCandle dailyCandle, double level, bool includeFutures = true)
        {
            var open = dailyCandle.Open;
            var max = open;

            double preHitMax = 0;
            var hit = false;
            var postHitMaxAdverseLevel = level;
            var postHitMaxFavourableLevel = level;

            foreach (var candle in candles)
            {
                if (includeFutures || candle.IsCash)
                {
                    if (level > open)
                    {
                        if (candle.BidLow < max)
                        {
                            max = candle.BidLow;
                        }

                        if (candle.AskHigh > level && !hit)
                        {
                            preHitMax = open - max;
                            hit = true;
                        }

                        if (candle.BidLow < postHitMaxFavourableLevel && hit)
                        {
                            postHitMaxFavourableLevel = candle.BidLow;
                        }

                        if (candle.AskHigh > postHitMaxAdverseLevel && hit)
                        {
                            postHitMaxAdverseLevel = candle.AskHigh;
                        }
                    }
                    else
                    {
                        if (candle.AskHigh > max)
                        {
                            max = candle.AskHigh;
                        }

                        if (candle.BidLow < level && !hit)
                        {
                            preHitMax = -(open - max);
                            hit = true;
                        }

                        if (candle.BidLow < postHitMaxAdverseLevel && hit)
                        {
                            postHitMaxAdverseLevel = candle.BidLow;
                        }

                        if (candle.AskHigh > postHitMaxFavourableLevel && hit)
                        {
                            postHitMaxFavourableLevel = candle.AskHigh;
                        }
                    }
                }
            }

            if (hit)
            {
                var postHitMaxFavourablePoints = Math.Abs(postHitMaxFavourableLevel - level);
                var postHitMaxAdversePoints = Math.Abs(postHitMaxAdverseLevel - level);

                return Option.Some((preHitMax, postHitMaxFavourablePoints, postHitMaxAdversePoints));
            }

            return Option.None<(double, double, double)>();
        }

        public static StrategyStats GetStrategyStats(IEnumerable<ITrade> trades)
        {
            double pointsTotal = 0;
            double cashProfit = 0;
            double biggestPointsWin = 0;
            double biggestCashWin = 0;
            var longestLosingStreak = 0;
            var longestWinningStreak = 0;
            var loseStreak = 0;
            var winStreak = 0;
            var wins = 0;
            var loses = 0;
            double pointsWinTotal = 0;
            double pointsLossTotal = 0;
            double cashWinTotal = 0;
            double cashLossTotal = 0;
            var previousTradeWon = false;

            foreach (var trade in trades)
            {
                trade.PointsProfit.IfExistsThen(x =>
                {
                    pointsTotal += x;
                    cashProfit += trade.CashProfit;

                    if (x > biggestPointsWin)
                    {
                        biggestPointsWin = x;
                    }

                    if (trade.CashProfit > biggestCashWin)
                    {
                        biggestCashWin = trade.CashProfit;
                    }

                    if (x > 0)
                    {
                        if (!previousTradeWon)
                        {
                            CheckLoseStreak();
                            loseStreak = 0;
                        }

                        wins++;
                        pointsWinTotal += x;
                        cashWinTotal += trade.CashProfit;
                        winStreak++;
                        previousTradeWon = true;
                    }
                    else if (x < 0)
                    {
                        if (previousTradeWon)
                        {
                            CheckWinStreak();
                            winStreak = 0;
                        }

                        loses++;
                        pointsLossTotal += x;
                        cashLossTotal += trade.CashProfit;
                        loseStreak++;
                        previousTradeWon = false;
                    }
                });
            }

            CheckLoseStreak();
            CheckWinStreak();
            var averagePointsWin = pointsWinTotal / wins;
            var averagePointsLoss = pointsLossTotal / loses;
            var averageCashWin = cashWinTotal / wins;
            var averageCashLoss = cashLossTotal / loses;

            var profitFactor = cashWinTotal / -cashLossTotal;

            return new StrategyStats(wins, loses, longestWinningStreak, longestLosingStreak, pointsTotal, cashProfit,
                biggestPointsWin, biggestCashWin, averagePointsWin, averagePointsLoss, averageCashWin, averageCashLoss,
                profitFactor);

            void CheckWinStreak()
            {
                if (winStreak > longestWinningStreak)
                {
                    longestWinningStreak = winStreak;
                }
            }

            void CheckLoseStreak()
            {
                if (loseStreak > longestLosingStreak)
                {
                    longestLosingStreak = loseStreak;
                }
            }
        }

    }
}
