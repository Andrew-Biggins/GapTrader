using System;
using System.Text;
using GapAnalyser.Interfaces;

namespace GapAnalyser.Candles
{
    public class DailyCandle : Candle, IDailyCandle
    {
        public Gap Gap { get; set; }

        public DailyCandle(DateTime date, double open, double high, double low, double close, double volume) : base(date, open, high,
            low, close, volume)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"{Date.ToShortDateString()} {Math.Round(Open)} {Math.Round(High)}" +
                                                 $" {Math.Round(Low)} {Math.Round(Close)}");
            

            return sb.ToString();
        }
    }
}
