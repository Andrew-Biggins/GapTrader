using GapTraderCore.Interfaces;
using System.Windows.Input;
using TradingSharedCore;
using TradingSharedCore.Interfaces;
using TradingSharedCore.ViewModels;

namespace GapTraderCore.ViewModels
{
    public sealed class GapTraderStrategyResultsStatsViewModel : StrategyResultsStatsViewModel
    {
        public ICommand ViewTradesCommand => new BasicCommand(ViewTrades);

        public GapTraderStrategyResultsStatsViewModel(IStrategy strategy, IGapTraderRunner runner,
            double accountStartSize) : base(strategy, runner, accountStartSize)
        {
            _runner = runner;
        }

        public GapTraderStrategyResultsStatsViewModel()
        {
        }

        private void ViewTrades()
        {
            _runner?.ShowTrades(this, Strategy);
        }

        private readonly IGapTraderRunner _runner;
    }
}
