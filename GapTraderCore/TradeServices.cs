using System;
using System.Collections.Generic;
using Foundations.Optional;
using GapTraderCore.Candles;
using GapTraderCore.Interfaces;
using GapTraderCore.Trades;

namespace GapTraderCore
{
    public enum TradeDirection
    {
        Long,
        Short,
        Both
    }

    internal static class TradeServices
    {
        internal static Optional<ITrade> AttemptTrade(IEnumerable<BidAskCandle> candles, double entry, double stop,
            double target, TimeSpan startTime, TimeSpan endTime, double risk, bool trialStop, double trialedStopSize)
        {
            var executed = false;
            var openTime = DateTime.MaxValue;
            var lastCandle = new BidAskCandle(new DateTime(1,1,1),1,1,1,1,1, Timezone.Uk);
            double openLevel = 0;
            var stopSize = Math.Abs(entry - stop);
            var positionSize = risk / stopSize;
            var direction = target > stop ? TradeDirection.Long : TradeDirection.Short;

            var high = double.NegativeInfinity;
            var low = double.PositiveInfinity;

            var originalStop = stop;
            var maximumAdverseExcursion = 0.0;
            var maximumFavourableExcursion = 0.0;

            var stopTrailed = false;

            foreach (var candle in candles)
            {
                if (candle.DateTime.TimeOfDay >= startTime && candle.DateTime.TimeOfDay <= endTime)
                {
                    if (direction == TradeDirection.Long)
                    {
                        if (trialStop && executed)
                        {
                            if (candle.BidOpen > high)
                            {
                                high = candle.BidOpen;
                            }
                            
                            // Trail stop only if in profit by at least the size of the trailed stop
                            if (high > stop + trialedStopSize && high > openLevel + trialedStopSize)
                            {
                                stop = candle.BidOpen - trialedStopSize;
                                stopTrailed = true;
                            }
                        }

                        // When not starting at the first candle of the day,
                        // if the first candle opens past the initial intended entry (positive slippage),
                        // enter the trade on the open of that candle and update stop level accordingly
                        if (candle.AskOpen <= entry && !executed)
                        {
                            openLevel = candle.AskOpen;
                            stop = openLevel - stopSize;
                            originalStop = openLevel - stopSize;
                            executed = true;
                            openTime = candle.DateTime;
                        }

                        if (candle.AskLow < entry && !executed)
                        {
                            openLevel = entry;
                            executed = true;
                            openTime = candle.DateTime;
                        }

                        if (executed)
                        {
                            var adverseExcursion = openLevel - candle.AskLow;

                            if (adverseExcursion > maximumAdverseExcursion)
                            {
                                maximumAdverseExcursion = adverseExcursion;
                            }

                            var favourableExcursion = candle.BidHigh - openLevel;

                            if (favourableExcursion > maximumAdverseExcursion)
                            {
                                maximumFavourableExcursion = favourableExcursion;
                            }

                            // Ensure MFA is 100% if target is hit 
                            if (candle.BidHigh > target)
                            {
                                maximumFavourableExcursion = target - openLevel;
                            }
                        }

                        if (candle.BidLow < stop && executed)
                        {
                            // Ensure MAE is 100% if full original stop is hit
                            if (!stopTrailed)
                            {
                                maximumAdverseExcursion = stopSize;
                            }

                            return Option.Some((ITrade) new Trade(entry, originalStop, target, openLevel, Option.Some(stop), openTime, Option.Some(candle.DateTime), positionSize, Option.Some(maximumAdverseExcursion), Option.Some(maximumFavourableExcursion)));
                        }

                        if (candle.BidHigh > target && executed)
                        {
                            return Option.Some((ITrade) new Trade(entry, originalStop, target, openLevel, Option.Some(target), openTime, Option.Some(candle.DateTime), positionSize, Option.Some(maximumAdverseExcursion), Option.Some(maximumFavourableExcursion)));
                        }
                    }
                    else if (direction == TradeDirection.Short)
                    {
                        if (trialStop && executed)
                        {
                            if (candle.AskOpen < low)
                            {
                                low = candle.AskOpen;
                            }

                            if (low < stop - trialedStopSize && low < openLevel - trialedStopSize)
                            {
                                stop = candle.AskOpen + trialedStopSize;
                                stopTrailed = true;
                            }
                        }

                        // See above comment on opposite direction trade
                        if (candle.BidOpen >= entry && !executed)
                        {
                            openLevel = candle.BidOpen;
                            stop = openLevel + stopSize;
                            originalStop = openLevel + stopSize;
                            executed = true;
                            openTime = candle.DateTime;
                        }

                        if (candle.BidHigh > entry && !executed)
                        {
                            openLevel = entry;
                            executed = true;
                            openTime = candle.DateTime;
                        }

                        if (executed)
                        {
                            var adverseExcursion = candle.BidHigh - openLevel;

                            if (adverseExcursion > maximumAdverseExcursion)
                            {
                                maximumAdverseExcursion = adverseExcursion;
                            }

                            var favourableExcursion = openLevel - candle.AskLow;

                            if (favourableExcursion > maximumAdverseExcursion)
                            {
                                maximumFavourableExcursion = favourableExcursion;
                            }

                            // Ensure MFA is 100% if target is hit 
                            if (candle.AskLow < target)
                            {
                                maximumFavourableExcursion = openLevel - target;
                            }
                        }

                        if (candle.AskHigh > stop && executed)
                        {
                            // Ensure MAE is 100% if full original stop is hit
                            if (!stopTrailed)
                            {
                                maximumAdverseExcursion = stopSize;
                            }

                            return Option.Some((ITrade) new Trade(entry, originalStop, target, openLevel, Option.Some(stop), openTime, Option.Some(candle.DateTime), positionSize, Option.Some(maximumAdverseExcursion), Option.Some(maximumFavourableExcursion)));
                        }

                        if (candle.AskLow < target && executed)
                        {
                            return Option.Some((ITrade) new Trade(entry, originalStop, target, openLevel, Option.Some(target), openTime, Option.Some(candle.DateTime), positionSize, Option.Some(maximumAdverseExcursion), Option.Some(maximumFavourableExcursion)));
                        }
                    }

                    lastCandle = candle;
                }
            }

            if (!executed)
            {
                return Option.None<ITrade>();
            }
            
            // When trade is executed but neither stop or target is hit close trade at close level of last candle
            double close;

            close = direction == TradeDirection.Long ? lastCandle.BidClose : lastCandle.AskClose;

            return Option.Some((ITrade) new Trade(entry, originalStop, target, entry, Option.Some(close), openTime, Option.Some(lastCandle.DateTime), positionSize, Option.Some(maximumAdverseExcursion), Option.Some(maximumFavourableExcursion)));
        }

        internal static double GetGapTradeEntryLevel(double gap, double open, double pointsEntry, bool fibEntry = false,
            FibonacciLevel fibLevelEntry = FibonacciLevel.Fifty)
        {
            var entry = gap > 0
                ? open + pointsEntry
                : open - pointsEntry;

            if (fibEntry)
            {
                entry += gap * GetFibRatio(fibLevelEntry);
            }

            return Math.Round(entry, 2);
        }

        internal static double GetGapTradeFixedPointsStopLevel(double gap, double entry, double stopSize)
        {
            var stop = gap > 0
                ? entry + stopSize
                : entry - stopSize;

            return stop;
        }

        internal static double GetGapTradePercentageStopLevel(double gap, double entry, double stopPercentage)
        {
            return entry + stopPercentage / 100 * gap;
        }

        internal static double GetGapTradeTargetLevel(double gap, double entry, double open, double pointsTarget,
            bool fibTarget = false, FibonacciLevel fibLevelTarget = FibonacciLevel.Fifty)
        {
            double target;

            if (fibTarget)
            {
                target = open - gap * GetFibRatio(fibLevelTarget);

                if (gap > 0)
                {
                    target -= pointsTarget;
                }
                else
                {
                    target += pointsTarget;
                }
            }
            else
            {
                target = gap > 0
                    ? entry - pointsTarget
                    : entry + pointsTarget;
            }

            return Math.Round(target, 2);
        }

        private static double GetFibRatio(FibonacciLevel level)
        {
            var ratio = (double)level / 1000;

            if (ratio > 1)
            {
                ratio -= 1;
            }

            return ratio;
        }
    }
}
