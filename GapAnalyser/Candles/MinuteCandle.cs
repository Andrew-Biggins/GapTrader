using System;
using System.Text;
using GapAnalyser.Interfaces;

namespace GapAnalyser.Candles
{
    public abstract class MinuteCandle : Candle, IMinuteCandle
    {
        public bool IsCash { get; set; }

        protected MinuteCandle(DateTime date, double open, double high, double low, double close, double volume) : base(date, open, high,
            low, close, volume)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder(
                $"{Date.ToShortDateString()} {Date.TimeOfDay} {Math.Round(Open)} {Math.Round(High)}" +
                $" {Math.Round(Low)} {Math.Round(Close)}  ");

            return sb.ToString();
        }

        protected virtual void CheckTime()
        {
            if (Date.TimeOfDay < CashClose.TimeOfDay && Date.TimeOfDay >= CashOpen.TimeOfDay)
            {
                IsCash = true;
            }
        }

        protected DateTime CashOpen;
        protected DateTime CashClose;
    }
}
