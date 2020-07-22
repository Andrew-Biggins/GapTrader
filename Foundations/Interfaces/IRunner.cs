using GapTraderCore.Interfaces;

namespace TradingSharedCore.Interfaces
{
    public interface IRunner
    {
        void Run(object sender, IRunnable runnable);

        void RunConcurrently(object sender, IRunnable runnable);

        void Run(object sender, Message message);

        bool RunForResult(object sender, Message message);

        void GetName(object sender, string title);

        void ShowGraphWindow(object sender);

        void ShowStrategyStatsWindow(object sender);
    }
}