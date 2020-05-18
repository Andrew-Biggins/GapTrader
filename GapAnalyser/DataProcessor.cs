using System;
using System.Collections.Generic;
using Foundations.Optional;
using GapAnalyser.Candles;

namespace GapAnalyser
{
    public static class DataProcessor
    {
        public static Optional<(double, double, double)> CompareMinuteData(IEnumerable<MinuteCandle> candles, double open, double level, bool includeFutures = true)
        {
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
                       if (candle.Low < max)
                       {
                           max = candle.Low;
                       }

                       if (candle.High > level && !hit)
                       {
                           preHitMax = open - max;
                           hit = true;
                       }

                       if (candle.Low < postHitMaxFavourableLevel && hit)
                       {
                           postHitMaxFavourableLevel = candle.Low;
                       }

                       if (candle.High > postHitMaxAdverseLevel && hit)
                       {
                           postHitMaxAdverseLevel = candle.High;
                       }
                   }
                   else
                   {
                       if (candle.High > max)
                       {
                           max = candle.High;
                       }

                       if (candle.Low < level && !hit)
                       {
                           preHitMax = -(open - max);
                           hit = true;
                       }

                       if (candle.Low < postHitMaxAdverseLevel && hit)
                       {
                           postHitMaxAdverseLevel = candle.Low;
                       }

                       if (candle.High > postHitMaxFavourableLevel && hit)
                       {
                           postHitMaxFavourableLevel = candle.High;
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
