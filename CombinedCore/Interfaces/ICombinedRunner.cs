using GapTraderCore.Interfaces;
using TradeJournalCore.Interfaces;

namespace CombinedCore.Interfaces
{
    public interface ICombinedRunner : ITradeJournalRunner, IGapTraderRunner
    {
        
    }
}