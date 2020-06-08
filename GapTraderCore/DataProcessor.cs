using System;
using System.Collections.Generic;
using Foundations.Optional;
using GapTraderCore.Candles;

namespace GapTraderCore
{
    public static class DataProcessor
    {
        public static Optional<(double, double, double)> CompareMinuteData(IEnumerable<BidAskCandle> candles, DailyCandle dailyCandle, double level, bool includeFutures = true)
        {
            var open = dailyCandle.Open;
            var max = open;

            double preHitMax = 0;
            var hit = false;
            var postHitMaxAdverseLevel = level;
            var postHitMaxFavourableLevel = level;

            foreach (var candle in candles)
            {
               if (includeFutures || candle.IsCash)
               {
                   if (level > open)
                   {
                       if (candle.BidLow < max)
                       {
                           max = candle.BidLow;
                       }

                       if (candle.AskHigh > level && !hit)
                       {
                           preHitMax = open - max;
                           hit = true;
                       }

                       if (candle.BidLow < postHitMaxFavourableLevel && hit)
                       {
                           postHitMaxFavourableLevel = candle.BidLow;
                       }

                       if (candle.AskHigh > postHitMaxAdverseLevel && hit)
                       {
                           postHitMaxAdverseLevel = candle.AskHigh;
                       }
                   }
                   else
                   {
                       if (candle.AskHigh > max)
                       {
                           max = candle.AskHigh;
                       }

                       if (candle.BidLow < level && !hit)
                       {
                           preHitMax = -(open - max);
                           hit = true;
                       }

                       if (candle.BidLow < postHitMaxAdverseLevel && hit)
                       {
                           postHitMaxAdverseLevel = candle.BidLow;
                       }

                       if (candle.AskHigh > postHitMaxFavourableLevel && hit)
                       {
                           postHitMaxFavourableLevel = candle.AskHigh;
                       }
                   }
               }
            }

            if (hit)
            {
                var postHitMaxFavourablePoints = Math.Abs(postHitMaxFavourableLevel - level);
                var postHitMaxAdversePoints = Math.Abs(postHitMaxAdverseLevel - level);

                return Option.Some((preHitMax, postHitMaxFavourablePoints, postHitMaxAdversePoints));
            }

            return Option.None<(double, double, double)>();
        }
    }
}
