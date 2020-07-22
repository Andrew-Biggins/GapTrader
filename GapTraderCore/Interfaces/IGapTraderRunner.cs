using TradingSharedCore.Interfaces;

namespace GapTraderCore.Interfaces
{
    public interface IGapTraderRunner : IRunner
    {
        void ShowLoadSavedDataWindow(object sender);

        void ShowSaveDataWindow(object sender);

        void ShowUploadNewDataWindow(object sender);

        void ShowTrades(object sender, IStrategy strategy);

    }
}