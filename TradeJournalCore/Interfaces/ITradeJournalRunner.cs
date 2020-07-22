using GapTraderCore.Interfaces;
using TradingSharedCore;
using TradingSharedCore.Interfaces;
using TradingSharedCore.Optional;

namespace TradeJournalCore.Interfaces
{
    public interface ITradeJournalRunner : IRunner
    {
        void GetTradeDetails(object sender);

        void GetStrategyDetails(object sender);

        Optional<string> OpenSaveDialog(object sender, string fileName, string filter);
    }
}