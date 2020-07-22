using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GapTraderCore.Candles;
using GapTraderCore.Interfaces;
using GapTraderCore.ViewModels;
using TradingSharedCore;
using TradingSharedCore.Optional;
using static GapTraderCore.DataProcessor;
using static TradingSharedCore.FibonacciServices;
using Option = TradingSharedCore.Optional.Option;

namespace GapTraderCore
{
    public class Market : IMarket
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public List<DailyCandle> DailyCandles { get; set; } = new List<DailyCandle>();

        public Dictionary<DateTime, List<BidAskCandle>> MinuteData { get; set; } =
            new Dictionary<DateTime, List<BidAskCandle>>();

        public List<Gap> UnfilledGaps { get; } = new List<Gap>();

        public Dictionary<FibonacciLevel, FibLevel> GapFibRetraceLevels { get; private set; }

        public Dictionary<FibonacciLevel, FibLevel> GapFibExtensionLevels { get; private set; }

        public DataDetails DataDetails { get; private set; }

        public bool IsUkData { get; set; }

        public string Name { get; protected set; }

        public Market(string name = "") 
        {
            
        }

        public void UpdateName(string name)
        {
            Name = name;
            PropertyChanged.Raise(this, nameof(Name));
        }

        public void DeriveDailyFromMinute(Del counter)
        {
            DailyCandles.Clear();

            foreach (var (date, minuteCandles) in MinuteData)
            {
                var cash = false;
                double high = 0;
                var low = double.PositiveInfinity;
                double open = 0;
                double close = 0;
                double previousClose = 0;

                var length = minuteCandles.Count;
                var i = 0;

                foreach (var candle in minuteCandles)
                {
                    if (!cash && candle.IsCash)
                    {
                        open = (candle.AskOpen - candle.BidOpen) / 2 + candle.BidOpen;
                        cash = true;
                    }

                    if (candle.AskHigh > high && candle.IsCash)
                    {
                        high = candle.AskHigh;
                    }

                    if (candle.BidLow < low && candle.IsCash)
                    {
                        low = candle.BidLow;
                    }

                    if (cash && !candle.IsCash)
                    {
                        close = previousClose;
                        cash = false;
                    }

                    previousClose = (candle.AskClose - candle.BidClose) / 2 + candle.BidClose;

                    // Handle cash only list
                    i++;
                    if (i == length && candle.IsCash)
                    {
                        close = (candle.AskClose - candle.BidClose) / 2 + candle.BidClose;
                    }
                }

                if (close != 0)
                {
                    var dailyCandle = new DailyCandle(date, open, high, low, close, 0);
                    DailyCandles.Add(dailyCandle);
                    counter();
                }
            }
        }

        public void ClearData()
        {
            DailyCandles.Clear();
            MinuteData.Clear();
            UnfilledGaps.Clear();
            GapFibRetraceLevels = NewFibRetraceDictionary();
            GapFibExtensionLevels = NewFibExtensionDictionary();
        }

        public void CalculateStats(bool ukData, Optional<double> previousClose)
        {
            IsUkData = ukData;

            CalculateGaps(previousClose);
            CalculateGapFillAndExtensionPercentages();
            UpdateGapFilledFlags();
            CalculateFiftyPercentGapFillLevels();
            CalculateGapLevelNextDayHitPercentages();
            CalculateGapFibLevelPreHitAdverseExcursions();
            GenerateDetails();
            PropertyChanged.Raise(this, string.Empty);
        }

        private void CalculateGaps(Optional<double> previousClose)
        {
            double pc = 0;

            previousClose.IfExistsThen(x =>
            {
                pc = x;
            }).IfEmpty(() =>
            {
                // First candle is only used for gap calculation on new data
                // (when previous close is not passed in) so removed from list
                pc = DailyCandles[0].Close;
                DailyCandles.RemoveAt(0);
            });

            double total = 0;

            foreach (var candle in DailyCandles)
            {
                var gap = new Gap(pc, candle.Open, candle.Date.Date);
                total += gap.AbsoluteGapPoints;
                candle.Gap = gap;
                pc = candle.Close;
            }

            _averageGapSize = total / DailyCandles.Count;
        }

        private void CalculateGapLevelNextDayHitPercentages()
        {
            var threeHundredAndSixtyOnePointEightCount = 0;
            var threeHundredCount = 0;
            var twoHundredAndSixtyOnePointEightCount = 0;
            var twoHundredAndFortyOnePointFourCount = 0;
            var twoHundredAndTwentySevenPointOneCount = 0;
            var twoHundredCount = 0;
            var oneHundredAndSixtyOnePointEightCount = 0;
            var oneHundredAndFortyOnePointFourCount = 0;
            var oneHundredAndTwentySevenPointOneCount = 0;
            var fivePointNineCount = 0;
            var elevenPointFourCount = 0;
            var twentyThreePointSixCount = 0;
            var thirtyEightPointTwoCount = 0;
            var fiftyCount = 0;
            var sixtyOnePointEightCount = 0;
            var seventyEightPointSixCount = 0;
            var eightyEightPointSixCount = 0;
            var ninetyFourPointOneCount = 0;
            var oneHundredCount = 0;

            foreach (var candle in DailyCandles)
            {
                if (candle.Gap.GapExtensionPercentage > 127.1)
                {
                    oneHundredAndTwentySevenPointOneCount++;
                }
                if (candle.Gap.GapExtensionPercentage > 141.4)
                {
                    oneHundredAndFortyOnePointFourCount++;
                }
                if (candle.Gap.GapExtensionPercentage > 161.8)
                {
                    oneHundredAndSixtyOnePointEightCount++;
                }
                if (candle.Gap.GapExtensionPercentage > 200)
                {
                    twoHundredCount++;
                }
                if (candle.Gap.GapExtensionPercentage > 227.1)
                {
                    twoHundredAndTwentySevenPointOneCount++;
                }
                if (candle.Gap.GapExtensionPercentage > 241.4)
                {
                    twoHundredAndFortyOnePointFourCount++;
                }
                if (candle.Gap.GapExtensionPercentage > 261.8)
                {
                    twoHundredAndSixtyOnePointEightCount++;
                }
                if (candle.Gap.GapExtensionPercentage > 300)
                {
                    threeHundredCount++;
                }
                if (candle.Gap.GapExtensionPercentage > 361.8)
                {
                    threeHundredAndSixtyOnePointEightCount++;
                }
                if (candle.Gap.GapFillPercentage > 5.9)
                {
                    fivePointNineCount++;
                }
                if (candle.Gap.GapFillPercentage > 11.4)
                {
                    elevenPointFourCount++;
                }
                if (candle.Gap.GapFillPercentage > 23.6)
                {
                    twentyThreePointSixCount++;
                }
                if (candle.Gap.GapFillPercentage > 38.2)
                {
                    thirtyEightPointTwoCount++;
                }
                if (candle.Gap.GapFillPercentage > 50)
                {
                    fiftyCount++;
                }
                if (candle.Gap.GapFillPercentage > 61.8)
                {
                    sixtyOnePointEightCount++;
                }
                if (candle.Gap.GapFillPercentage > 78.6)
                {
                    seventyEightPointSixCount++;
                }
                if (candle.Gap.GapFillPercentage > 88.6)
                {
                    eightyEightPointSixCount++;
                }
                if (candle.Gap.GapFillPercentage > 94.1)
                {
                    ninetyFourPointOneCount++;
                }
                if (candle.Gap.GapFillPercentage >= 100)
                {
                    oneHundredCount++;
                }
            }

            var multiplier = 100 / (double)DailyCandles.Count;

            GapFibExtensionLevels[FibonacciLevel.OneHundredAndTwentySevenPointOne].NextDayHitPercentage = oneHundredAndTwentySevenPointOneCount * multiplier;
            GapFibExtensionLevels[FibonacciLevel.OneHundredAndFortyOnePointFour].NextDayHitPercentage = oneHundredAndFortyOnePointFourCount * multiplier;
            GapFibExtensionLevels[FibonacciLevel.OneHundredAndSixtyOnePointEight].NextDayHitPercentage = oneHundredAndSixtyOnePointEightCount * multiplier;
            GapFibExtensionLevels[FibonacciLevel.TwoHundred].NextDayHitPercentage = twoHundredCount * multiplier;
            GapFibExtensionLevels[FibonacciLevel.TwoHundredAndTwentySevenPointOne].NextDayHitPercentage = twoHundredAndTwentySevenPointOneCount * multiplier;
            GapFibExtensionLevels[FibonacciLevel.TwoHundredAndFortyOnePointFour].NextDayHitPercentage = twoHundredAndFortyOnePointFourCount * multiplier;
            GapFibExtensionLevels[FibonacciLevel.TwoHundredAndSixtyOnePointEight].NextDayHitPercentage = twoHundredAndSixtyOnePointEightCount * multiplier;
            GapFibExtensionLevels[FibonacciLevel.ThreeHundred].NextDayHitPercentage = threeHundredCount * multiplier;
            GapFibExtensionLevels[FibonacciLevel.ThreeHundredAndSixtyOnePointEight].NextDayHitPercentage = threeHundredAndSixtyOnePointEightCount * multiplier;

            GapFibRetraceLevels[FibonacciLevel.FivePointNine].NextDayHitPercentage = fivePointNineCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.ElevenPointFour].NextDayHitPercentage = elevenPointFourCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.TwentyThreePointSix].NextDayHitPercentage = twentyThreePointSixCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.ThirtyEightPointTwo].NextDayHitPercentage = thirtyEightPointTwoCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.Fifty].NextDayHitPercentage = fiftyCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.SixtyOnePointEight].NextDayHitPercentage = sixtyOnePointEightCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.SeventyEightPointSix].NextDayHitPercentage = seventyEightPointSixCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.EightyEightPointSix].NextDayHitPercentage = eightyEightPointSixCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.NinetyFourPointOne].NextDayHitPercentage = ninetyFourPointOneCount * multiplier;
            GapFibRetraceLevels[FibonacciLevel.OneHundred].NextDayHitPercentage = oneHundredCount * multiplier;
        }

        private void CalculateGapFibLevelPreHitAdverseExcursions() // todo rename this
        {
            var retraces = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            for (var i = 0; i < 19; i++)
            {
                var hitCount = 0;
                double preHitMaxTotal = 0;
                double postHitMaxFavourableTotal = 0;
                double postHitMaxAdverseTotal = 0;
                double highestPreHitMax = 0;
                double highestPostHitMaxFavourable = 0;
                double highestPostHitMaxAdverse = 0;
                var highestPreHitMaxDate = DateTime.MinValue;
                var highestPostHitMaxFavourableDate = DateTime.MinValue;
                var highestPostHitMaxAdverseDate = DateTime.MinValue;

                foreach (var dailyCandle in DailyCandles)
                {
                    var level = i < 10
                        ? dailyCandle.Open - dailyCandle.Gap.GapPoints * ((double) retraces[i] / 1000)
                        : dailyCandle.Open + dailyCandle.Gap.GapPoints * ((double) retraces[i] / 1000 - 1);


                    if (MinuteData.TryGetValue(dailyCandle.Date.Date, out var minuteCandles))
                    {
                        var excursions = CompareMinuteData(minuteCandles, dailyCandle, level, false);

                        excursions.IfExistsThen(x =>
                        {
                            var (preHitMax, postHitMaxFavourable, postHitMaxAdverse) = x;

                            if (preHitMax > highestPreHitMax)
                            {
                                highestPreHitMax = preHitMax;
                                highestPreHitMaxDate = dailyCandle.Date;
                            }

                            if (postHitMaxFavourable > highestPostHitMaxFavourable)
                            {
                                highestPostHitMaxFavourable = postHitMaxFavourable;
                                highestPostHitMaxFavourableDate = dailyCandle.Date;
                            }

                            if (postHitMaxAdverse > highestPostHitMaxAdverse)
                            {
                                highestPostHitMaxAdverse = postHitMaxAdverse;
                                highestPostHitMaxAdverseDate = dailyCandle.Date;
                            }

                            hitCount++;
                            preHitMaxTotal += preHitMax;
                            postHitMaxFavourableTotal += postHitMaxFavourable;
                            postHitMaxAdverseTotal += postHitMaxAdverse;
                        });
                    }
                }

                if (GapFibRetraceLevels.TryGetValue(retraces[i], out var retraceLevel)) // todo refactor this
                {
                    retraceLevel.AveragePreHitAdverseExcursion = preHitMaxTotal / hitCount;
                    retraceLevel.HighestPreHitAdverseExcursion = highestPreHitMax;
                    retraceLevel.DateOfHighestPreHitAdverseExcursion = highestPreHitMaxDate;

                    retraceLevel.AveragePostHitFavourableExcursion = postHitMaxFavourableTotal / hitCount;
                    retraceLevel.HighestPostHitFavourableExcursion = highestPostHitMaxFavourable;
                    retraceLevel.DateOfHighestPostHitFavourableExcursion = highestPostHitMaxFavourableDate;

                    retraceLevel.AveragePostHitAdverseExcursion = postHitMaxAdverseTotal / hitCount;
                    retraceLevel.HighestPostHitAdverseExcursion = highestPostHitMaxAdverse;
                    retraceLevel.DateOfHighestPostHitAdverseExcursion = highestPostHitMaxAdverseDate;
                }

                if (GapFibExtensionLevels.TryGetValue(retraces[i], out var extensionLevel))
                {
                    extensionLevel.AveragePreHitAdverseExcursion = preHitMaxTotal / hitCount;
                    extensionLevel.HighestPreHitAdverseExcursion = highestPreHitMax;
                    extensionLevel.DateOfHighestPreHitAdverseExcursion = highestPreHitMaxDate;

                    extensionLevel.AveragePostHitFavourableExcursion = postHitMaxFavourableTotal / hitCount;
                    extensionLevel.HighestPostHitFavourableExcursion = highestPostHitMaxFavourable;
                    extensionLevel.DateOfHighestPostHitFavourableExcursion = highestPostHitMaxFavourableDate;

                    extensionLevel.AveragePostHitAdverseExcursion = postHitMaxAdverseTotal / hitCount;
                    extensionLevel.HighestPostHitAdverseExcursion = highestPostHitMaxAdverse;
                    extensionLevel.DateOfHighestPostHitAdverseExcursion = highestPostHitMaxAdverseDate;
                }
            }
        }

        private void CalculateGapFillAndExtensionPercentages()
        {
            foreach (var candle in DailyCandles)
            {
                var gfp = candle.Gap.GapPoints < 0
                    ? (candle.High - candle.Open) / (candle.Gap.GapPoints * -1) * 100
                    : (candle.Open - candle.Low) / candle.Gap.GapPoints * 100;

                candle.Gap.GapFillPercentage = gfp > 100 ? 100 : gfp;

                var gep = candle.Gap.GapPoints < 0
                    ? (candle.Low - candle.Open) / (candle.Gap.GapPoints * -1) * 100 + 100
                    : (candle.High - candle.Open) / candle.Gap.GapPoints * 100 + 100;

                candle.Gap.GapExtensionPercentage = gep < 0 ? 0 : gep;
            }
        }

        private void UpdateGapFilledFlags()
        {
            for (var i = 0; i < DailyCandles.Count; i++)
            {
                if (DailyCandles[i].Gap.GapFillPercentage == 100)
                {
                    DailyCandles[i].Gap.HasGapBeenFilled = true;
                    DailyCandles[i].Gap.GapFillDate = Option.Some(DailyCandles[i].Date);
                }
                else
                {
                    if (DailyCandles[i].Gap.GapPoints > 0)
                    {
                        for (var j = i + 1; j < DailyCandles.Count; j++)
                        {
                            if (DailyCandles[j].Low < DailyCandles[i].Open - DailyCandles[i].Gap.GapPoints)
                            {
                                DailyCandles[i].Gap.HasGapBeenFilled = true;
                                DailyCandles[i].Gap.GapFillDate = Option.Some(DailyCandles[j].Date);
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (var j = i + 1; j < DailyCandles.Count; j++)
                        {
                            if (DailyCandles[j].High > DailyCandles[i].Open - DailyCandles[i].Gap.GapPoints)
                            {
                                DailyCandles[i].Gap.HasGapBeenFilled = true;
                                DailyCandles[i].Gap.GapFillDate = Option.Some(DailyCandles[j].Date);
                                break;
                            }
                        }
                    }
                }

                if (!DailyCandles[i].Gap.HasGapBeenFilled)
                {
                    UnfilledGaps.Add(DailyCandles[i].Gap);
                }
            }
        }

        private void CalculateFiftyPercentGapFillLevels()
        {
            foreach (var candle in DailyCandles)
            {
                candle.Gap.FiftyPercentGapFillLevel =
                    candle.Open - candle.Gap.GapPoints * 0.5;
            }
        }

        private void GenerateDetails()
        {
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;

            foreach (var dailyCandle in DailyCandles.Where(dailyCandle => MinuteData.ContainsKey(dailyCandle.Date.Date)))
            {
                startDate = dailyCandle.Date;
                break;
            }

            for (var i = DailyCandles.Count - 1; i > 0; i--)
            {
                if (MinuteData.ContainsKey(DailyCandles[i].Date.Date))
                {
                    endDate = DailyCandles[i].Date;
                    break;
                }
            }

            var high = double.MinValue;
            var low = double.MaxValue;
            var date = startDate;
            var highTime = DateTime.MinValue;
            var lowTime = DateTime.MinValue;

            while (date <= endDate)
            {
                if (MinuteData.TryGetValue(date.Date, out var minuteCandles))
                {
                    foreach (var candle in minuteCandles.Where(candle => candle.IsCash))
                    {
                        if (candle.AskHigh > high)
                        {
                            high = candle.AskHigh;
                            highTime = candle.DateTime;
                        }

                        if (candle.BidLow < low)
                        {
                            low = candle.BidLow;
                            lowTime = candle.DateTime;
                        }
                    }
                }

                date = date.AddDays(1);
            }

            var openTime = new TimeSpan(14,30,00);
            var closeTime = new TimeSpan(21, 00,00);

            if (IsUkData)
            {
                openTime = new TimeSpan(8, 00, 00);
                closeTime = new TimeSpan(16, 30, 00);
            }

            DataDetails = new DataDetails(startDate, endDate, high, highTime, low, lowTime, _averageGapSize, openTime,
                closeTime);
        }

        private double _averageGapSize;
    }
}
