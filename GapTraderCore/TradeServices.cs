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
            double target, TimeSpan startTime, TimeSpan endTime)
        {
            var executed = false;
            var openTime = DateTime.MaxValue;
            var lastCandle = new BidAskCandle(new DateTime(1,1,1),1,1,1,1,1, Timezone.Uk);

            var stopSize = Math.Abs(entry - stop);
            double entrySlippage = 0;

            var direction = target > stop ? TradeDirection.Long : TradeDirection.Short;

            foreach (var candle in candles)
            {
                if (candle.DateTime.TimeOfDay >= startTime && candle.DateTime.TimeOfDay <= endTime)
                {
                    if (direction == TradeDirection.Long)
                    {
                        // When not starting at the first candle of the day,
                        // if the first candle opens past the initial intended entry (positive slippage),
                        // enter the trade on the open of that candle and update stop level accordingly
                        if (candle.AskOpen <= entry && !executed)
                        {
                            entrySlippage = entry - candle.AskOpen; 
                            entry = candle.AskOpen;
                            stop = entry - stopSize;
                            executed = true;
                            openTime = candle.DateTime;
                            continue; // Prevent opening and closing trade on the same candle -- means slippage is possible?
                        }

                        if (candle.AskLow < entry && !executed)
                        {
                            executed = true;
                            openTime = candle.DateTime;
                            continue;
                        }

                        if (candle.BidLow < stop && executed)
                        {
                            return Option.Some((ITrade) new Trade(stop, target, entry, stop, openTime, candle.DateTime,
                                -(entry - stop), entrySlippage, direction));
                        }

                        if (candle.BidHigh > target && executed)
                        {
                            return Option.Some((ITrade) new Trade(stop, target, entry, target, openTime, candle.DateTime,
                                target - entry, entrySlippage, direction));
                        }
                    }
                    else if (direction == TradeDirection.Short)
                    {
                        // See above comment on opposite direction trade
                        if (candle.BidOpen >= entry && !executed)
                        {
                            entrySlippage = candle.BidOpen - entry;
                            entry = candle.BidOpen;
                            stop = entry + stopSize;
                            executed = true;
                            openTime = candle.DateTime;
                            continue;
                        }

                        if (candle.BidHigh > entry && !executed)
                        {
                            executed = true;
                            openTime = candle.DateTime;
                            continue;
                        }

                        if (candle.AskHigh > stop && executed)
                        {
                            return Option.Some((ITrade) new Trade(stop, target, entry, stop, openTime, candle.DateTime,
                                -(stop - entry), entrySlippage, direction));
                        }

                        if (candle.AskLow < target && executed)
                        {
                            return Option.Some((ITrade) new Trade(stop, target, entry, target, openTime, candle.DateTime,
                                entry - target, entrySlippage, direction));
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

            return target > stop
                ? Option.Some((ITrade) new Trade(stop, target, entry, close, openTime, lastCandle.DateTime,
                    close - entry, entrySlippage, direction))
                : Option.Some((ITrade) new Trade(stop, target, entry, close, openTime, lastCandle.DateTime,
                    entry - close, entrySlippage, direction));
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
