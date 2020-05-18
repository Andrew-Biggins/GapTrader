namespace GapAnalyser
{
    public struct StrategyStats
    {
        public int TradeCount { get; }

        public int Wins { get; }

        public int Loses { get; }

        public int LongestWinningStreak { get; }

        public int LongestLosingStreak { get; }

        public double Profit { get; }

        public double BiggestWin { get; }

        public double AverageWin { get; }

        public double AverageLoss { get; }

        public double WinProbability { get; }

        public double ProfitFactor { get; }

        public double Expectancy { get; }

        public StrategyStats(int wins, int loses, int longestWinningStreak, int longestLosingStreak,
            double profit, double biggestWin, double averageWin, double averageLoss, double profitFactor)
        {
            TradeCount = wins + loses;
            Wins = wins;
            Loses = loses;
            LongestWinningStreak = longestWinningStreak;
            LongestLosingStreak = longestLosingStreak;
            Profit = profit;
            BiggestWin = biggestWin;
            AverageWin = averageWin;
            AverageLoss = averageLoss;
            ProfitFactor = profitFactor;

            WinProbability = (double) wins / (wins + loses);

            Expectancy = WinProbability * AverageWin - (1-WinProbability) * AverageLoss;
        }
    }
}