using System;

namespace GapTraderCore.Candles
{
    public class BidAskCandle
    {
        public DateTime DateTime { get; private set; }

        public double BidOpen { get; }

        public double BidHigh { get; }

        public double BidLow { get; }

        public double BidClose { get; }

        public double Volume { get; }

        public double AskOpen { get; private set; }

        public double AskHigh { get; private set; }

        public double AskLow { get; private set; }

        public double AskClose { get; private set; }

        public bool IsCash { get; set; }

        public BidAskCandle(DateTime date, double bidOpen, double bidHigh, double bidLow, double bidClose, double volume, Timezone timezone)
        {
            DateTime = date;
            BidOpen = bidOpen;
            BidHigh = bidHigh;
            BidLow = bidLow;
            BidClose = bidClose;
            Volume = volume;
            _timezone = timezone;
            CheckTime();
        }

        public void AddAskPrices(double open, double high, double low, double close)
        {
            AskOpen = open;
            AskHigh = high;
            AskLow = low;
            AskClose = close;
        }

        private void CheckTime()
        {
            if (_timezone == Timezone.Us)
            {
                AdjustTimeForDayLightSavingsDifference();
            }

            var (open, close) = TimeServices.GetOpenCloseTimes(_timezone);

            if (DateTime.TimeOfDay < close && DateTime.TimeOfDay >= open)
            {
                IsCash = true;
            }
        }

        private void AdjustTimeForDayLightSavingsDifference()
        {
            if (DateTime.Date > new DateTime(2017, 10, 29) && DateTime.Date < new DateTime(2017, 11, 6) ||
                DateTime.Date > new DateTime(2018, 3, 11) && DateTime.Date < new DateTime(2018, 3, 28) ||
                DateTime.Date > new DateTime(2018, 10, 28) && DateTime.Date < new DateTime(2018, 11, 4) ||
                DateTime.Date > new DateTime(2019, 3, 10) && DateTime.Date < new DateTime(2019, 3, 29) ||
                DateTime.Date > new DateTime(2019, 10, 27) && DateTime.Date < new DateTime(2019, 11, 3) ||
                DateTime.Date > new DateTime(2020, 3, 8) && DateTime.Date < new DateTime(2020, 3, 29) ||
                DateTime.Date > new DateTime(2020, 10, 25) && DateTime.Date < new DateTime(2020, 11, 1) ||
                DateTime.Date > new DateTime(2021, 3, 14) && DateTime.Date < new DateTime(2021, 3, 28) ||
                DateTime.Date > new DateTime(2021, 10, 31) && DateTime.Date < new DateTime(2021, 11, 7))
            {
                DateTime = DateTime.AddHours(1);
            }
        }

        private readonly Timezone _timezone;
    }
}
