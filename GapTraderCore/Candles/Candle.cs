using System;
using GapTraderCore.Interfaces;

namespace GapTraderCore.Candles
{
    public abstract class Candle : ICandle
    {
        public DateTime Date { get; protected set; }

        public double Open { get; }

        public double High { get; }

        public double Low { get; }

        public double Close { get; }

        public double Volume { get; }

        protected Candle(DateTime date, double open, double high, double low, double close, double volume)
        {
            Date = date;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
        }
    }
}
