using GapTraderCore.Interfaces;
using System.Collections.Generic;

namespace TradingSharedCore
{
    public static class StrategyStatsGenerator
    {
        public static StrategyStats GetStrategyStats(List<ITrade> trades, double startBalance)
        {
            double pointsTotal = 0;
            double cashProfit = 0;
            double biggestPointsWin = 0;
            double biggestCashWin = 0;
            double biggestPointsLoss = 0;
            double biggestCashLoss = 0;
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

            double drawdownTotal = 0;
            var maeTotal = 0.0;
            var maeCount = 0;
            var mfeTotal = 0.0;
            var mfeCount = 0;
            var realisedProfitTotal = 0.0;
            var unrealisedProfitPointsTotal = 0.0;
            var unrealisedProfitCashTotal = 0.0;
            var resultInRTotal = 0.0;
            var riskRewardRatioTotal = 0.0;

            foreach (var trade in trades)
            {
                riskRewardRatioTotal += trade.RiskRewardRatio;

                trade.PointsProfit.IfExistsThen(x =>
                {
                    pointsTotal += x;

                    trade.CashProfit.IfExistsThen(y =>
                    {
                        cashProfit += y;
                        if (y > biggestCashWin)
                        {
                            biggestCashWin = y;
                        }

                        if (y < biggestCashLoss)
                        {
                            biggestCashLoss = y;
                        }
                    });

                    if (x > biggestPointsWin)
                    {
                        biggestPointsWin = x;
                    }

                    if (x < biggestPointsLoss)
                    {
                        biggestPointsLoss = x;
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

                        trade.CashProfit.IfExistsThen(y =>
                        {
                            cashWinTotal += y;
                        });

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

                        trade.CashProfit.IfExistsThen(y =>
                        {
                            cashLossTotal += y;
                        });

                        loseStreak++;
                        previousTradeWon = false;
                    }

                    trade.MaximumAdverseExcursionPoints.IfExistsThen(y => { maeTotal += y; maeCount++; });
                    trade.MaximumAdverseExcursionPercentageOfStop.IfExistsThen(y => { drawdownTotal += y; });

                    trade.MaximumFavourableExcursionPoints.IfExistsThen(y => { mfeTotal += y; mfeCount++; });
                    trade.PointsProfitPercentageOfMaximumFavourableExcursion.IfExistsThen(
                        y => { realisedProfitTotal += y; });

                    trade.UnrealisedProfitPoints.IfExistsThen(y => { unrealisedProfitPointsTotal += y; });
                    trade.UnrealisedProfitCash.IfExistsThen(y => { unrealisedProfitCashTotal += y; });

                    trade.ResultInR.IfExistsThen(y => { resultInRTotal += y; });
                });
            }

            CheckLoseStreak();
            CheckWinStreak();
            var averagePointsWin = pointsWinTotal / wins;
            var averagePointsLoss = pointsLossTotal / loses;
            var averageCashWin = cashWinTotal / wins;
            var averageCashLoss = cashLossTotal / loses;
            var averageMae = maeTotal / maeCount;
            var averageDrawdown = drawdownTotal / maeCount;
            var averageMfe = mfeTotal / mfeCount;
            var averageRealisedProfitPercentage = realisedProfitTotal / mfeCount;
            var averageUnrealisedProfitPoints = unrealisedProfitPointsTotal / mfeCount;
            var averageUnrealisedProfitCash = unrealisedProfitCashTotal / mfeCount;
            var averageResultInR = resultInRTotal / (wins + loses);
            var averageRiskRewardRatio = riskRewardRatioTotal / trades.Count;

            var profitFactor = cashWinTotal / -cashLossTotal;

            return new StrategyStats(wins, loses, longestWinningStreak, longestLosingStreak, pointsTotal, cashProfit,
                biggestPointsWin, biggestCashWin, averagePointsWin, averagePointsLoss, averageCashWin, averageCashLoss,
                profitFactor, averageMae, averageDrawdown, averageMfe, averageRealisedProfitPercentage,
                averageUnrealisedProfitPoints, averageUnrealisedProfitCash, averageResultInR, averageRiskRewardRatio,
                biggestCashLoss, biggestPointsLoss, startBalance);

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
