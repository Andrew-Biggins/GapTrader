using System;
using TradingSharedCore;
using TradingSharedCore.Interfaces;
using TradingSharedCore.Optional;
using TradingSharedCore.Strategies;
using TradingSharedCore.Trades;

namespace TradeJournalCore.Trades
{
    public class JournalTrade : Trade, IJournalTrade
    {
        public ISelectableStrategy Strategy { get; }

        public ISelectable Market { get; }

        public JournalTrade(double strategyEntry, double stop, double target, double openLevel, Optional<double> closeLevel,
            DateTime openTime, Optional<DateTime> closeTime, ISelectable market, ISelectableStrategy strategy,
            double size, Optional<double> maximumAdverseExcursion, Optional<double> maximumFavourableExcursion) : base(strategyEntry, stop, target, openLevel,
            closeLevel, openTime, closeTime, size, maximumAdverseExcursion, maximumFavourableExcursion)
        {
            Strategy = strategy;
            Market = market;
        }

        public JournalTrade(ISavedTrade savedTrade) : base(savedTrade)
        {
            Strategy = new Strategy<FibonacciLevel, FibonacciLevel>(savedTrade.StrategyName, savedTrade.StrategyShortName);
            Market = new SelectableMarket(savedTrade.MarketName);
        }
    }
}