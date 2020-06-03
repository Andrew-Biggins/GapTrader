﻿using System.Collections.Generic;

namespace GapAnalyser.Interfaces
{
    public interface IStrategy
    {
        StrategyStats Stats { get; }

        object Entry { get; }

        object Target { get; }

        double Stop { get; }

        List<ITrade> Trades { get; }

        string Title { get; }
    }
}