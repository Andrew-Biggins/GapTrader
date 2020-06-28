namespace GapTraderCore.Interfaces
{
    public interface IRunner
    {
        void ShowTrades(object sender, IStrategy strategy);

        void Run(object sender, IRunnable runnable);

        void RunConcurrently(object sender, IRunnable runnable);

        void Run(object sender, Message message);

        bool RunForResult(object sender, Message message);

        void GetName(object sender, string title);

        void ShowLoadSavedDataWindow(object sender);

        void ShowSaveDataWindow(object sender);

        void ShowGraphWindow(object sender);

        void ShowUploadNewDataWindow(object sender);

        void GetTradeDetails(object sender);

        void GetStrategyDetails(object sender);
    }
}