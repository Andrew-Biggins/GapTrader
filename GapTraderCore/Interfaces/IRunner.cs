namespace GapTraderCore.Interfaces
{
    public interface IRunner
    {
        void ShowTrades(object sender, IStrategy strategy);

        void Run(object sender, IRunnable runnable);

        void RunConcurrently(object sender, IRunnable runnable);

        void Run(object sender, Message message);

        bool RunForResult(object sender, Message message);

        void GetSaveName(object sender);

        void ShowLoadSavedDataWindow(object sender);

        void ShowUploadNewDataWindow(object sender);

        // Optional<string> OpenSaveDialog(object sender, string fileName, string filter);
    }
}