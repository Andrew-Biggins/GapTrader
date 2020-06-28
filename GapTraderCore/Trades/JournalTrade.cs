using System;
using Foundations.Optional;
using GapTraderCore.Interfaces;
using GapTraderCore.Strategies;

namespace GapTraderCore.Trades
{
    public class JournalTrade : Trade, IJournalTrade
    {
        public ISelectableStrategy Strategy { get; }

        public ISelectable Market { get; }

        public JournalTrade(double strategyEntry, double stop, double target, double openLevel, Optional<double> closeLevel,
            DateTime openTime, Optional<DateTime> closeTime, ISelectable market, ISelectableStrategy strategy, double size) : base(
            strategyEntry, stop, target, openLevel, closeLevel, openTime, closeTime, size)
        {
            Strategy = strategy;
            Market = market;
        }

        public JournalTrade(SavedTrade savedTrade) : base(savedTrade)
        {
            Strategy = new Strategy<FibonacciLevel, FibonacciLevel>(savedTrade.StrategyName, savedTrade.StrategyShortName);
            Market = new Market(savedTrade.MarketName);
        }
    }
}