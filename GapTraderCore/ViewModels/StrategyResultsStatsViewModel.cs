using System.Windows.Input;
using Foundations;
using GapTraderCore.Interfaces;

namespace GapTraderCore.ViewModels
{
    public sealed class StrategyResultsStatsViewModel : BindableBase
    {
        public string TradeCount { get; private set; } = string.Empty;

        public string Wins { get; private set; } = string.Empty;

        public string Loses { get; private set; } = string.Empty;

        public string LongestWinningStreak { get; private set; } = string.Empty;

        public string LongestLosingStreak { get; private set; } = string.Empty;

        public string PointsProfit { get; private set; } = string.Empty;

        public string CashProfit { get; private set; } = string.Empty;

        public string BiggestCashWin { get; private set; } = string.Empty;

        public string BiggestPointsWin { get; private set; } = string.Empty;

        public string BiggestCashLoss { get; private set; } = string.Empty;

        public string BiggestPointsLoss { get; private set; } = string.Empty;

        public string AverageWin { get; private set; } = string.Empty;

        public string AverageLoss { get; private set; } = string.Empty;

        public string AverageRiskRewardRatio { get; private set; } = string.Empty;

        public string AverageResultInR { get; private set; } = string.Empty;

        public string AverageMaximumAdverseExcursion { get; private set; } = string.Empty;

        public string AverageMaximumFavourableExcursion { get; private set; } = string.Empty;

        public string AverageDrawdown { get; private set; } = string.Empty;

        public string AverageRealisedProfitPercentage { get; private set; } = string.Empty;

        public string AverageUnrealisedProfit { get; private set; } = string.Empty;

        public string WinProbability { get; private set; } = string.Empty;

        public string ProfitFactor { get; private set; } = string.Empty;

        public string Expectancy { get; private set; } = string.Empty;

        public ICommand ViewTradesCommand => new BasicCommand(ViewTrades);

        public ICommand ViewGraphCommand => new BasicCommand(ViewGraph);

        public ICommand MoreDetailsCommand => new BasicCommand(() => _runner?.ShowStrategyStatsWindow(this));

        public bool HasResults { get; }

        public string Name { get; }

        public string Gain { get; private set; }

        public StrategyResultsStatsViewModel()
        {
            
        }

        public StrategyResultsStatsViewModel(StrategyStats stats, string name = "Strategy Statistics")
        {
            _accountStartSize = stats.StartBalance;
            Name = name;
            AssignData(stats);
        }

        public StrategyResultsStatsViewModel(StrategyStats stats, IRunner runner, string name = "Strategy Statistics")
        {
            _accountStartSize = stats.StartBalance;
            _runner = runner;
            Name = name;
            AssignData(stats);
            HasResults = true;
        }

        public StrategyResultsStatsViewModel(IStrategy strategy, IRunner runner, double accountStartSize)
        {
            _runner = runner;
            _strategy = strategy;
            _accountStartSize = accountStartSize;
            HasResults = true;
            Name = strategy.Name;
            AssignData(strategy.Stats);
        }

        public void UpdateAccountStartSize(double accountStartSize)
        {
            _accountStartSize = accountStartSize;
            var gain = _cashProfit / _accountStartSize;

            Gain = $"{gain:P}";
            RaisePropertyChanged(nameof(Gain));
        }

        private void AssignData(StrategyStats stats)
        {
            _cashProfit = stats.CashProfit;
            TradeCount = $"{stats.TradeCount}";
            Wins = $"{stats.Wins}";
            Loses = $"{stats.Loses}";
            LongestWinningStreak = $"{stats.LongestWinningStreak}";
            LongestLosingStreak = $"{stats.LongestLosingStreak}";
            PointsProfit = $"{stats.PointsTotal:N1}";
            CashProfit = $"{stats.CashProfit:C}";
            BiggestCashWin = $"{stats.BiggestCashWin:C}";
            BiggestPointsWin = $"{stats.BiggestPointsWin:N1}";
            BiggestCashLoss = $"{stats.BiggestCashLoss:C}";
            BiggestPointsLoss = $"{stats.BiggestPointsLoss:N1}";

            AverageMaximumAdverseExcursion = double.IsNaN(stats.AverageMaximumAdverseExcursion)
                ? "---"
                : $"{stats.AverageMaximumAdverseExcursion:N1}";

            AverageMaximumFavourableExcursion = double.IsNaN(stats.AverageMaximumFavourableExcursion)
                ? "---"
                : $"{stats.AverageMaximumFavourableExcursion:N1}";

            AverageDrawdown = double.IsNaN(stats.AverageDrawdown)
                ? "---"
                : $"{stats.AverageDrawdown:P}";

            AverageRealisedProfitPercentage = double.IsNaN(stats.AverageRealisedProfitPercentage)
                ? "---"
                : $"{stats.AverageRealisedProfitPercentage:P}";

            AverageRiskRewardRatio = double.IsNaN(stats.AverageRiskRewardRatio)
                ? "---"
                : $"{stats.AverageRiskRewardRatio:N1}";

            AverageResultInR = double.IsNaN(stats.AverageResultInR)
                ? "---"
                : $"{stats.AverageResultInR:N1}";

            AverageWin = double.IsNaN(stats.AveragePointsWin) || double.IsNaN(stats.AverageCashWin)
                ? "---"
                : $"{stats.AveragePointsWin:N1} Points / {stats.AverageCashWin:C}";

            AverageLoss = double.IsNaN(stats.AveragePointsLoss) || double.IsNaN(stats.AverageCashLoss)
                ? "---"
                : $"{stats.AveragePointsLoss:N1} Points / {stats.AverageCashLoss:C}";

            AverageUnrealisedProfit = $"{stats.AverageUnrealisedProfitPoints:N1} Points / {stats.AverageUnrealisedProfitCash:C}";

            WinProbability = double.IsNaN(stats.WinProbability)
                ? "---"
                : $"{stats.WinProbability:P}";

            ProfitFactor = double.IsNaN(stats.ProfitFactor) ||
                           double.IsInfinity(stats.ProfitFactor)
                ? "---"
                : $"{stats.ProfitFactor:N1}";

            Expectancy = double.IsNaN(stats.PointsExpectancy) || double.IsNaN(stats.CashExpectancy)
                ? "---"
                : $"{stats.PointsExpectancy:N1} Points / {stats.CashExpectancy:C}";

            var gain = stats.CashProfit / _accountStartSize;

            Gain = $"{gain:P}";
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
        private double _cashProfit;
        private double _accountStartSize;

    }
}
