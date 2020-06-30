using System.Windows.Input;
using Foundations;
using GapTraderCore.Interfaces;

namespace GapTraderCore.ViewModels
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

        public ICommand ViewTradesCommand => new BasicCommand(ViewTrades);

        public ICommand ViewGraphCommand => new BasicCommand(ViewGraph);

        public ICommand MoreDetailsCommand => new BasicCommand(() => _runner?.ShowStrategyStatsWindow(this));

        public bool HasResults { get; }

        public StrategyResultsStatsViewModel()
        {
            
        }

        public StrategyResultsStatsViewModel(StrategyStats stats)
        {
            TradeCount = $"{stats.TradeCount}";
            Wins = $"{stats.Wins}";
            Loses = $"{stats.Loses}";
            LongestWinningStreak = $"{stats.LongestWinningStreak}";
            LongestLosingStreak = $"{stats.LongestLosingStreak}";
            PointsProfit = $"{stats.PointsTotal:N1}";
            CashProfit = $"{stats.CashProfit:N1}";
            BiggestWin = $"{stats.BiggestCashWin:N1}";

            AverageLoss = double.IsNaN(stats.AveragePointsLoss)
                ? "---"
                : $"{stats.AveragePointsLoss:N1}";

            AverageWin = double.IsNaN(stats.AverageCashWin)
                ? "---"
                : $"{stats.AverageCashWin:N1}";

            WinProbability = double.IsNaN(stats.WinProbability)
                ? "---"
                : $"{stats.WinProbability:P}";

            ProfitFactor = double.IsNaN(stats.ProfitFactor) ||
                           double.IsInfinity(stats.ProfitFactor)
                ? "---"
                : $"{stats.ProfitFactor:N1}";

            Expectancy = double.IsNaN(stats.Expectancy)
                ? "---"
                : $"{stats.Expectancy:N1}";
        }

        public StrategyResultsStatsViewModel(IStrategy strategy, IRunner runner, double accountStartSize)
        {
            _runner = runner;
            _strategy = strategy;
            _accountStartSize = accountStartSize;
            HasResults = true;

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

        private void ViewTrades()
        {
            _runner?.ShowTrades(this, _strategy);
        }

        private void ViewGraph()
        {
            _runner?.ShowGraphWindow(new GraphWindowViewModel(_accountStartSize, _strategy.Trades));
        }

        private readonly IStrategy _strategy;
        private readonly IRunner _runner;
        private readonly double _accountStartSize;
    }
}
