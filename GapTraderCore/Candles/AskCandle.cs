using System;

namespace GapTraderCore.Candles
{
    public class AskCandle
    {
        public DateTime DateTime { get; private set; }

        public double AskOpen { get; }

        public double AskHigh { get; }

        public double AskLow { get; }

        public double AskClose { get; }

        public AskCandle(DateTime date, double bidOpen, double bidHigh, double bidLow, double bidClose, Timezone timezone)
        {
            DateTime = date;
            AskOpen = bidOpen;
            AskHigh = bidHigh;
            AskLow = bidLow;
            AskClose = bidClose;
            _timezone = timezone;
        }

        private void CheckTime(bool dayLightSavingAdjust)
        {
            if (_timezone == Timezone.Us && dayLightSavingAdjust)
            {
                AdjustTimeForDayLightSavingsDifference();
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
