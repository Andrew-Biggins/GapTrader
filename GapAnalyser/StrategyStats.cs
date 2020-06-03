namespace GapAnalyser
{
    public struct StrategyStats
    {
        public int TradeCount { get; }

        public int Wins { get; }

        public int Loses { get; }

        public int LongestWinningStreak { get; }

        public int LongestLosingStreak { get; }

        public double PointsTotal { get; }

        public double CashProfit { get; }

        public double BiggestPointsWin { get; }

        public double BiggestCashWin { get; }

        public double AveragePointsWin { get; }

        public double AveragePointsLoss { get; }

        public double AverageCashWin { get; }

        public double AverageCashLoss { get; }

        public double WinProbability { get; }

        public double ProfitFactor { get; }

        public double Expectancy { get; }

        public StrategyStats(int wins, int loses, int longestWinningStreak, int longestLosingStreak,
            double pointsTotal, double cashProfit, double biggestPointsWin, double biggestCashWin,
            double averagePointsWin, double averagePointsLoss, double averageCashWin, double averageCashLoss,
            double profitFactor)
        {
            TradeCount = wins + loses;
            Wins = wins;
            Loses = loses;
            LongestWinningStreak = longestWinningStreak;
            LongestLosingStreak = longestLosingStreak;
            PointsTotal = pointsTotal;
            CashProfit = cashProfit;
            BiggestPointsWin = biggestPointsWin;
            BiggestCashWin = biggestCashWin;
            AveragePointsWin = averagePointsWin;
            AveragePointsLoss = averagePointsLoss;
            AverageCashWin = averageCashWin;
            AverageCashLoss = averageCashLoss;


            ProfitFactor = profitFactor;

            WinProbability = (double) wins / (wins + loses);

            Expectancy = WinProbability * AveragePointsWin - (1-WinProbability) * AveragePointsLoss; //todo wrong!?
        }
    }
}