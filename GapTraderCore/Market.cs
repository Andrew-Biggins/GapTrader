using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Foundations;
using Foundations.Optional;
using GapTraderCore.Candles;
using GapTraderCore.Interfaces;
using GapTraderCore.ViewModels;
using static GapTraderCore.DataProcessor;
using static GapTraderCore.FibonacciServices;

namespace GapTraderCore
{
    public class Market : IMarket
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public List<DailyCandle> DailyCandles { get; set; } = new List<DailyCandle>();

        public Dictionary<DateTime, List<BidAskCandle>> MinuteData { get; set; } =
            new Dictionary<DateTime, List<BidAskCandle>>();

        public List<Gap> UnfilledGaps { get; } = new List<Gap>();

        public Dictionary<FibonacciLevel, FibLevel> GapFibLevels { get; private set; }

        public DataDetails DataDetails { get; private set; }

        public bool IsUkData { get; set; }

        public void DeriveDailyFromMinute(Del counter)
        {
            foreach (var (date, minuteCandles) in MinuteData)
            {
                var cash = false;
                double high = 0;
                var low = double.PositiveInfinity;
                double open = 0;
                double close = 0;
                double previousClose = 0;

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
            GapFibLevels = NewFibRetraceDictionary();
        }

        public void CalculateStats(bool ukData, Optional<double> previousClose)
        {
            IsUkData = ukData;

            CalculateGaps(previousClose);
            CalculateGapFillPercentages();
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

            previousClose.IfExistsThen( x =>
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

            GapFibLevels[FibonacciLevel.FivePointNine].NextDayHitPercentage = fivePointNineCount * multiplier;
            GapFibLevels[FibonacciLevel.ElevenPointFour].NextDayHitPercentage = elevenPointFourCount * multiplier;
            GapFibLevels[FibonacciLevel.TwentyThreePointSix].NextDayHitPercentage = twentyThreePointSixCount * multiplier;
            GapFibLevels[FibonacciLevel.ThirtyEightPointTwo].NextDayHitPercentage = thirtyEightPointTwoCount * multiplier;
            GapFibLevels[FibonacciLevel.Fifty].NextDayHitPercentage = fiftyCount * multiplier;
            GapFibLevels[FibonacciLevel.SixtyOnePointEight].NextDayHitPercentage = sixtyOnePointEightCount * multiplier;
            GapFibLevels[FibonacciLevel.SeventyEightPointSix].NextDayHitPercentage = seventyEightPointSixCount * multiplier;
            GapFibLevels[FibonacciLevel.EightyEightPointSix].NextDayHitPercentage = eightyEightPointSixCount * multiplier;
            GapFibLevels[FibonacciLevel.NinetyFourPointOne].NextDayHitPercentage = ninetyFourPointOneCount * multiplier;
            GapFibLevels[FibonacciLevel.OneHundred].NextDayHitPercentage = oneHundredCount * multiplier;
        }

        private void CalculateGapFibLevelPreHitAdverseExcursions()
        {
            var retraces = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            for (var i = 0; i < 10; i++)
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
                    var level = dailyCandle.Open - dailyCandle.Gap.GapPoints * ((double)retraces[i] / 1000);

                    if (MinuteData.TryGetValue(dailyCandle.Date, out var minuteCandles))
                    {
                        // todo this isn't working. Post MFA on Dax
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

                if (GapFibLevels.TryGetValue(retraces[i], out var fibLevel))
                {
                    fibLevel.AveragePreHitAdverseExcursion = preHitMaxTotal / hitCount;
                    fibLevel.HighestPreHitAdverseExcursion = highestPreHitMax;
                    fibLevel.DateOfHighestPreHitAdverseExcursion = highestPreHitMaxDate;

                    fibLevel.AveragePostHitFavourableExcursion = postHitMaxFavourableTotal / hitCount;
                    fibLevel.HighestPostHitFavourableExcursion = highestPostHitMaxFavourable;
                    fibLevel.DateOfHighestPostHitFavourableExcursion = highestPostHitMaxFavourableDate;

                    fibLevel.AveragePostHitAdverseExcursion = postHitMaxAdverseTotal / hitCount;
                    fibLevel.HighestPostHitAdverseExcursion = highestPostHitMaxAdverse;
                    fibLevel.DateOfHighestPostHitAdverseExcursion = highestPostHitMaxAdverseDate;
                }
            }
        }

        private void CalculateGapFillPercentages()
        {
            foreach (var candle in DailyCandles)
            {
                var gfp = candle.Gap.GapPoints < 0
                    ? (candle.High - candle.Open) / (candle.Gap.GapPoints * -1) * 100
                    : (candle.Open - candle.Low) / candle.Gap.GapPoints * 100;

                candle.Gap.GapFillPercentage = gfp > 100 ? 100 : gfp;
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

            foreach (var dailyCandle in DailyCandles.Where(dailyCandle => MinuteData.ContainsKey(dailyCandle.Date)))
            {
                startDate = dailyCandle.Date;
                break;
            }

            for (var i = DailyCandles.Count - 1; i > 0; i--)
            {
                if (MinuteData.ContainsKey(DailyCandles[i].Date))
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
                if (MinuteData.TryGetValue(date, out var minuteCandles))
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
