using System;
using Foundations.Optional;
using GapTraderCore.Interfaces;

namespace GapTraderCore.Trades
{
    public class JournalTrade : Trade, IJournalTrade
    {
        public IStrategy Strategy { get; }

        public IMarket Market { get; }

        public JournalTrade(double strategyEntry, double stop, double target, double openLevel, Optional<double> closeLevel,
            DateTime openTime, Optional<DateTime> closeTime, IMarket market, IStrategy strategy, double size) : base(
            strategyEntry, stop, target, openLevel, closeLevel, openTime, closeTime, size)
        {
            Strategy = strategy;
            Market = market;
        }
    }
}