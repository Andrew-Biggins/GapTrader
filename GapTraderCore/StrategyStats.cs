namespace GapTraderCore
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

        public double BiggestPointsLoss { get; }

        public double BiggestCashLoss { get; }

        public double AveragePointsWin { get; }

        public double AveragePointsLoss { get; }

        public double AverageCashWin { get; }

        public double AverageCashLoss { get; }

        public double WinProbability { get; }

        public double ProfitFactor { get; }

        public double CashExpectancy { get; }

        public double PointsExpectancy { get; }

        public double AverageDrawdown { get; }

        public double AverageMaximumAdverseExcursion { get; }

        public double AverageMaximumFavourableExcursion { get; }

        public double AverageRiskRewardRatio { get; }

        public double AverageResultInR { get; }

        public double AverageRealisedProfitPercentage { get; }

        public double AverageUnrealisedProfitPoints { get; }

        public double AverageUnrealisedProfitCash { get; }

        public string Expectancy { get; }

        public double StartBalance { get; }

        public StrategyStats(int wins, int loses, int longestWinningStreak, int longestLosingStreak,
            double pointsTotal, double cashProfit, double biggestPointsWin, double biggestCashWin,
            double averagePointsWin, double averagePointsLoss, double averageCashWin, double averageCashLoss,
            double profitFactor, double averageMae, double averageDrawdown, double averageMfe, 
            double averageRealisedProfitPercentage, double averageUnrealisedProfitPoints, 
            double averageUnrealisedProfitCash, double averageResultInR, double averageRiskRewardRatio,
            double biggestCashLoss, double biggestPointsLoss, double startBalance)
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
            BiggestCashLoss = biggestCashLoss;
            BiggestPointsLoss = biggestPointsLoss;
            AveragePointsWin = averagePointsWin;
            AveragePointsLoss = averagePointsLoss;
            AverageCashWin = averageCashWin;
            AverageCashLoss = averageCashLoss;
            ProfitFactor = profitFactor;
            AverageMaximumAdverseExcursion = averageMae;
            AverageDrawdown = averageDrawdown;
            AverageMaximumFavourableExcursion = averageMfe;
            AverageRealisedProfitPercentage = averageRealisedProfitPercentage;
            AverageUnrealisedProfitPoints = averageUnrealisedProfitPoints;
            AverageUnrealisedProfitCash = averageUnrealisedProfitCash;
            AverageResultInR = averageResultInR;
            AverageRiskRewardRatio = averageRiskRewardRatio;
            StartBalance = startBalance;

            WinProbability = (double) wins / (wins + loses);

            CashExpectancy = WinProbability * AverageCashWin - (1-WinProbability) * -AverageCashLoss;
            PointsExpectancy = WinProbability * AveragePointsWin - (1-WinProbability) * -AveragePointsLoss;

            Expectancy = double.IsNaN(PointsExpectancy) || double.IsNaN(CashExpectancy)
                ? "---"
                : $"{PointsExpectancy:N1} Points / {CashExpectancy:C}";
        }
    }
}