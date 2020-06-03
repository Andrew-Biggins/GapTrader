using System;
using System.Xml.Serialization;

namespace GapAnalyser.Candles
{
    public sealed class UsMinuteCandle : MinuteCandle
    {
        public UsMinuteCandle(DateTime date, double open, double high, double low, double close, double volume) : base(
            date, open, high, low, close, volume)
        {
            CashClose = new DateTime(1, 1, 1, 21, 0, 0);
            CashOpen = new DateTime(1, 1, 1, 14, 30, 0);
            CheckTime();
        }

        protected override void CheckTime()
        {
            // Adjust time for different daylight saving time periods
            //if (Date.Date > new DateTime(2019, 10, 27) && Date.Date < new DateTime(2019, 11, 3) ||
            //    Date.Date > new DateTime(2020, 3, 8) && Date.Date < new DateTime(2020, 3, 29))
            //{
            //    Date = Date.AddHours(1);
            //}

            base.CheckTime();
        }
    }
}
