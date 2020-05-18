using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Foundations;
using Foundations.Optional;
using GapAnalyser.Candles;
using GapAnalyser.Interfaces;
using static GapAnalyser.CsvServices;
using static GapAnalyser.FibonacciServices;
using static GapAnalyser.DataProcessor;

namespace GapAnalyser
{
    public class Market : IMarket
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public List<DailyCandle> DailyCandles { get; } = new List<DailyCandle>();

        public Dictionary<DateTime, List<MinuteCandle>> MinuteData { get; } =
            new Dictionary<DateTime, List<MinuteCandle>>();

        public List<Gap> UnfilledGaps { get; } = new List<Gap>();

        public Dictionary<FibonacciLevel, FibLevel> GapFibLevels { get; private set; }

        public DataDetails DataDetails { get; private set; }

        public bool UkData { get; private set; }

        public void ReadInData(string filePath)
        {
            using var csvParser = SetUpParser(filePath);
            double firstClose = 0;
            var adding = false;
            var firstCloseRead = false;

            while (!csvParser.EndOfData)
            {
                var candle = ParseDailyCandle(csvParser.ReadFields());

                if (candle.Date >= _firstMinuteDataDate && firstCloseRead)
                {
                    DailyCandles.Add(candle);
                    adding = true;
                }

                if (!adding)
                {
                    firstClose = candle.Close;
                }

                firstCloseRead = true;
            }

            ProcessGaps(firstClose);
        }

        public void ReadInMinuteData(string filePath, bool ukData)
        {
            UkData = ukData;

            using var csvParser = SetUpParser(filePath);

            var date = DateTime.MinValue;
            var mc = new List<MinuteCandle>();

            var firstDateRead = false;

            while (!csvParser.EndOfData)
            {
                var candle = UkData
                    ? ParseUkMinuteCandle(csvParser.ReadFields())
                    : ParseUsMinuteCandle(csvParser.ReadFields());

                if (Math.Abs(candle.Volume) > 0)
                {
                    if (!firstDateRead)
                    {
                        _firstMinuteDataDate = candle.Date;
                        firstDateRead = true;
                    }

                    if (candle.Date.Date != date.Date)
                    {
                        if (mc.Count != 0)
                        {
                            MinuteData.Add(date.Date, mc);
                        }

                        mc = new List<MinuteCandle>();
                    }

                    date = candle.Date.Date;
                    mc.Add(candle);
                }
            }

            if (mc.Count != 0)
            {
                MinuteData.Add(date.Date, mc);
            }
        }

        public void DeriveDailyFromMinute()
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
                        open = candle.Open;
                        cash = true;
                    }

                    if (candle.High > high && candle.IsCash)
                    {
                        high = candle.High;
                    }

                    if (candle.Low < low && candle.IsCash)
                    {
                        low = candle.Low;
                    }

                    if (cash && !candle.IsCash)
                    {
                        close = previousClose;
                        cash = false;
                    }

                    previousClose = candle.Close;
                }

                if (close != 0)
                {
                    var dailyCandle = new DailyCandle(date, open, high, low, close, 0);
                    DailyCandles.Add(dailyCandle);
                }
            }

            // First candle is only used for gap calculation so removed from list
            var firstClose = DailyCandles[0].Close;
            DailyCandles.RemoveAt(0);

            ProcessGaps(firstClose);
        }

        public void CalculateGapLevelNextDayHitPercentages()
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

        public void ClearData()
        {
            DailyCandles.Clear();
            MinuteData.Clear();
            UnfilledGaps.Clear();
            GapFibLevels = NewFibRetraceDictionary();
        }

        private void ProcessGaps(double firstClose)
        {
            CalculateGaps(firstClose);
            CalculateGapFillPercentages();
            UpdateGapFilledFlags();
            CalculateFiftyPercentGapFillLevels();
            CalculateGapLevelNextDayHitPercentages();
        }

        private void CalculateGaps(double previousClose)
        {
            double total = 0;

            foreach (var candle in DailyCandles)
            {
                var gap = new Gap(previousClose, candle.Open, candle.Date.Date);
                total += gap.AbsoluteGapPoints;
                candle.Gap = gap;
                previousClose = candle.Close;
            }

            _averageGapSize = total / DailyCandles.Count;
        }

        public void CalculateGapFibLevelPreHitAdverseExcursions()
        {
            //foreach (FibonacciLevel fib in Enum.GetValues(typeof(FibonacciLevel)))
            //{
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
                        var phae = CompareMinuteData(minuteCandles, dailyCandle.Open, level, false);

                        phae.IfExistsThen(x =>
                        {
                            var preHitMax = x.Item1;
                            var postHitMaxFavourable = x.Item2;
                            var postHitMaxAdverse = x.Item3;

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

            GenerateDetails();
            PropertyChanged.Raise(this, string.Empty);
        }

        private void CalculateGapFillPercentages()
        {
            foreach (var candle in DailyCandles)
            {
                var gfp = candle.Gap.GapPoints < 0
                    ? (candle.High - candle.Open) / (candle.Gap.GapPoints * -1) * 100
                    : (candle.Open - candle.Low) / candle.Gap.GapPoints * 100;

                candle.Gap.GapFillPercentage = gfp > 100 ? 100 : gfp;

                Debug.WriteLine(candle.Gap.GapFillPercentage);
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
            for (var i = 1; i < DailyCandles.Count; i++)
            {
                DailyCandles[i].Gap.FiftyPercentGapFillLevel =
                    DailyCandles[i].Open - DailyCandles[i].Gap.GapPoints * 0.5;
            }
        }

        private void GenerateDetails()
        {
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;

            foreach (var dailyCandle in DailyCandles)
            {
                if (MinuteData.ContainsKey(dailyCandle.Date))
                {
                    startDate = dailyCandle.Date;
                    break;
                }
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
                        if (candle.High > high)
                        {
                            high = candle.High;
                            highTime = candle.Date;
                        }

                        if (candle.Low < low)
                        {
                            low = candle.Low;
                            lowTime = candle.Date;
                        }
                    }
                }

                date = date.AddDays(1);
            }

            DataDetails = new DataDetails(startDate, endDate, high, highTime, low, lowTime, _averageGapSize);
        }

        private DateTime _firstMinuteDataDate;
        private double _averageGapSize;
    }
}
