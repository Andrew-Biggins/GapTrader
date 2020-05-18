using System;

namespace GapAnalyser.Candles
{
    public sealed class UkMinuteCandle : MinuteCandle
    {
        public UkMinuteCandle(DateTime date, double open, double high, double low, double close, double volume) : base(
            date, open, high, low, close, volume)
        {
            CashClose = new DateTime(1, 1, 1, 16, 30, 0);
            CashOpen = new DateTime(1, 1, 1, 8, 0, 0);
            CheckTime();
        }
    }
}
