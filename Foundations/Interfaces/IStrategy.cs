using System.Collections.Generic;
using GapTraderCore.Interfaces;
using TradingSharedCore.Optional;

namespace TradingSharedCore.Interfaces
{
    public interface IStrategy
    {
        StrategyStats Stats { get; }

        object Entry { get; }

        object Target { get; }

        double Stop { get; }

        List<ITrade> Trades { get; }

        string Name { get; }

        Optional<double> TrailedStopSize { get; }
    }
}