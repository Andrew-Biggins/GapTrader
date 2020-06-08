using System;

namespace GapTraderCore.Interfaces
{
    public interface ICandle
    {
        DateTime Date { get; }

        double Open { get; }

        double High { get; }

        double Low { get;  }

        double Close { get; }
    }
}