using System;

namespace TradingSharedCore.Interfaces
{
    public interface ISavedTrade
    {
        string StrategyName { get; }
        string StrategyShortName { get; }
        string MarketName { get; }
        double StrategyEntryLevel { get; }
        double StopLevel { get; }
        double Target { get; }
        double OpenLevel { get; }
        double CloseLevel { get; }
        double MaximumAdverseExcursion { get; }
        double MaximumFavourableExcursion { get; }
        DateTime OpenTime { get; }
        DateTime CloseTime { get; }
        double Size { get; }
    }
}