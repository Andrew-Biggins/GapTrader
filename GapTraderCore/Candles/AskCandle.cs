using System;

namespace GapTraderCore.Candles
{
    public class AskCandle
    {
        public DateTime DateTime { get; }

        public double AskOpen { get; }

        public double AskHigh { get; }

        public double AskLow { get; }

        public double AskClose { get; }

        public AskCandle(DateTime date, double bidOpen, double bidHigh, double bidLow, double bidClose)
        {
            DateTime = date;
            AskOpen = bidOpen;
            AskHigh = bidHigh;
            AskLow = bidLow;
            AskClose = bidClose;
        }
    }
}
