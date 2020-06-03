using GapAnalyser.Interfaces;

namespace GapAnalyser.ViewModels
{
    public sealed class StrategyResultsStatsViewModel
    {
        public string TradeCount { get; } = string.Empty;

        public string Wins { get; } = string.Empty;

        public string Loses { get; } = string.Empty;

        public string LongestWinningStreak { get; } = string.Empty;

        public string LongestLosingStreak { get; } = string.Empty;

        public string PointsProfit { get; } = string.Empty;

        public string CashProfit { get; } = string.Empty;

        public string BiggestWin { get; } = string.Empty;

        public string AverageWin { get; } = string.Empty;

        public string AverageLoss { get; } = string.Empty;

        public string WinProbability { get; } = string.Empty;

        public string ProfitFactor { get; } = string.Empty;

        public string Expectancy { get; } = string.Empty;

        public StrategyResultsStatsViewModel()
        {
            
        }

        public StrategyResultsStatsViewModel(IStrategy strategy)
        {
            TradeCount = $"{strategy.Stats.TradeCount}";
            Wins = $"{strategy.Stats.Wins}";
            Loses = $"{strategy.Stats.Loses}";
            LongestWinningStreak = $"{strategy.Stats.LongestWinningStreak}";
            LongestLosingStreak = $"{strategy.Stats.LongestLosingStreak}";
            PointsProfit = $"{strategy.Stats.PointsTotal:N1}";
            CashProfit = $"{strategy.Stats.CashProfit:N1}";
            BiggestWin = $"{strategy.Stats.BiggestCashWin:N1}";

            AverageLoss = double.IsNaN(strategy.Stats.AveragePointsLoss) 
                ? "---" 
                : $"{strategy.Stats.AveragePointsLoss:N1}";

            AverageWin = double.IsNaN(strategy.Stats.AverageCashWin)
                ? "---"
                : $"{strategy.Stats.AverageCashWin:N1}";

            WinProbability = double.IsNaN(strategy.Stats.WinProbability)
                ? "---"
                : $"{strategy.Stats.WinProbability:P}";

            ProfitFactor = double.IsNaN(strategy.Stats.ProfitFactor) ||
                           double.IsInfinity(strategy.Stats.ProfitFactor)
                ? "---"
                : $"{strategy.Stats.ProfitFactor:N1}";

            Expectancy = double.IsNaN(strategy.Stats.Expectancy)
                ? "---"
                : $"{strategy.Stats.Expectancy:N1}";
        }
    }

}
