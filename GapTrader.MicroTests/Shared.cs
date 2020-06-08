namespace GapTrader.MicroTests
{
    internal static class Shared
    {
        internal static IMarket MarketWithTestData
        {
            get
            {
                var market = Substitute.For<IMarket>();
                market.MinuteData.Returns(MinuteTestData);
                market.DailyCandles.Returns(DailyTestData);
                return market;
            }
        }

        private static List<DailyCandle> DailyTestData
        {
            get
            {
                var dailyCandles = new List<DailyCandle>()
                {
                    new DailyCandle(new DateTime(2020,4, 24), 800, 901, 700, 700, 1),
                    new DailyCandle(new DateTime(2020,4, 25), 600, 900, 550, 900, 1),
                    new DailyCandle(new DateTime(2020,4, 26), 1200, 2200, 1100, 1500, 1),
                };

                dailyCandles = AddGaps(dailyCandles);

                return dailyCandles;
            }
        }

        private static Dictionary<DateTime, List<MinuteCandle>> MinuteTestData
        {
            get
            {
                var minuteData = new Dictionary<DateTime, List<MinuteCandle>>();
                minuteData.Add(new DateTime(2020,4,24), DayOne);
                minuteData.Add(new DateTime(2020,4,25), DayTwo);
                minuteData.Add(new DateTime(2020,4,26), DayThree);
                return minuteData;
            }
        }

        private static List<MinuteCandle> DayOne
        {
            get
            {
                var dayData = new List<MinuteCandle>()
                {
                    new UsMinuteCandle(new DateTime(2020,4, 24,14,35,00), 800, 800, 750, 750, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 24,14,36,00), 750, 750, 700, 700, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 24,14,37,00), 700, 800, 700, 800, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 24,14,38,00), 800, 901, 800, 901, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 24,14,39,00), 901, 901, 800, 800, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 24,14,40,00), 800, 800, 700, 700, 0.0016)
                };

                return dayData;
            }
        }

        private static List<MinuteCandle> DayTwo
        {
            get
            {
                var dayData = new List<MinuteCandle>()
                {
                    new UsMinuteCandle(new DateTime(2020,4, 25,14,35,00), 600, 650, 550, 650, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 25,14,36,00), 650, 751, 650, 751, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 25,14,37,00), 751, 900, 751, 900, 0.0016),
                };

                return dayData;
            }
        }

        private static List<MinuteCandle> DayThree
        {
            get
            {
                var dayData = new List<MinuteCandle>()
                {
                    new UsMinuteCandle(new DateTime(2020,4, 26,14,35,00), 1200, 2200, 1200, 2200, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 26,14,36,00), 2200, 2200, 1100, 1100, 0.0016),
                    new UsMinuteCandle(new DateTime(2020,4, 26,14,37,00), 1100, 1500, 1100, 1500, 0.0016),
                };

                return dayData;
            }
        }

        private static List<DailyCandle> AddGaps(List<DailyCandle> dailyCandles)
        {
            double previousClose = 500;

            foreach (var candle in dailyCandles)
            {
                var gap = new Gap(previousClose, candle.Open, candle.Date.Date);
                candle.Gap = gap;
                previousClose = candle.Close;
            }

            return dailyCandles;
        }
    }
}
