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

            foreach (var candle in candles)
            {
                if (candle.DateTime.TimeOfDay >= startTime && candle.DateTime.TimeOfDay <= endTime)
                {
                    if (direction == TradeDirection.Long)
                    {
                        if (trialStop)
                        {
                            if (candle.BidOpen > high)
                            {
                                high = candle.BidOpen;
                            }

                            if (executed && high > stop + trialedStopSize)
                            {
                                stop = candle.BidOpen - trialedStopSize;
                            }
                        }

                        // When not starting at the first candle of the day,
                        // if the first candle opens past the initial intended entry (positive slippage),
                        // enter the trade on the open of that candle and update stop level accordingly
                        if (candle.AskOpen <= entry && !executed)
                        {
                            openLevel = candle.AskOpen;
                            stop = entry - stopSize;
                            originalStop = entry - stopSize;
                            executed = true;
                            openTime = candle.DateTime;
                        }

                        if (candle.AskLow < entry && !executed)
                        {
                            openLevel = entry;
                            executed = true;
                            openTime = candle.DateTime;
                        }

                        if (candle.BidLow < stop && executed)
                        {
                            return Option.Some((ITrade) new Trade(entry, originalStop, target, openLevel, Option.Some(stop), openTime, Option.Some(candle.DateTime), positionSize));
                        }

                        if (candle.BidHigh > target && executed)
                        {
                            return Option.Some((ITrade) new Trade(entry, originalStop, target, openLevel, Option.Some(target), openTime, Option.Some(candle.DateTime), positionSize));
                        }
                    }
                    else if (direction == TradeDirection.Short)
                    {
                        if (trialStop)
                        {
                            if (candle.AskOpen < low)
                            {
                                low = candle.AskOpen;
                            }

                            if (executed && low < stop - trialedStopSize)
                            {
                                stop = candle.AskOpen + trialedStopSize;
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

                        if (candle.AskHigh > stop && executed)
                        {
                            return Option.Some((ITrade) new Trade(entry, originalStop, target, openLevel, Option.Some(stop), openTime, Option.Some(candle.DateTime), positionSize));
                        }

                        if (candle.AskLow < target && executed)
                        {
                            return Option.Some((ITrade) new Trade(entry, originalStop, target, openLevel, Option.Some(target), openTime, Option.Some(candle.DateTime), positionSize));
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
            var close = direction == TradeDirection.Long ? lastCandle.BidClose : lastCandle.AskClose;

            return Option.Some((ITrade) new Trade(entry, originalStop, target, entry, Option.Some(close), openTime, Option.Some(lastCandle.DateTime), positionSize));
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
