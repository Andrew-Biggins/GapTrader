using System;
using System.Collections.Generic;
using System.Text;
using Foundations.Optional;
using GapAnalyser.Candles;
using GapAnalyser.Interfaces;

namespace GapAnalyser
{
    internal static class TradeServices
    {
        internal static Optional<ITrade> AttemptTrade(IEnumerable<MinuteCandle> candles, double entry, double stop,
            double target, TimeSpan startTime, TimeSpan endTime)
        {
            var executed = false;
            var openTime = DateTime.MaxValue;
            MinuteCandle lastCandle = new UkMinuteCandle(new DateTime(1,1,1),1,1,1,1,1 );

            var stopSize = Math.Abs(entry - stop);

            foreach (var candle in candles)
            {
                if (candle.Date.TimeOfDay >= startTime && candle.Date.TimeOfDay <= endTime)
                {
                    if (target > stop)
                    {
                        // When not starting at the first candle of the day,
                        // if the first candle opens past the initial intended entry (positive slippage),
                        // enter the trade on the open of that candle and update stop level accordingly
                        if (candle.Open <= entry && !executed)
                        {
                            entry = candle.Open;
                            stop = entry - stopSize;
                            executed = true;
                            openTime = candle.Date;
                            continue; // Prevent opening and closing trade on the same candle -- means slippage is possible?
                        }

                        if (candle.Low < entry && !executed)
                        {
                            executed = true;
                            openTime = candle.Date;
                            continue;
                        }

                        if (candle.Low < stop && executed)
                        {
                            return Option.Some((ITrade) new Trade(stop, target, entry, stop, openTime, candle.Date,
                                -(entry - stop)));
                        }

                        if (candle.High > target && executed)
                        {
                            return Option.Some((ITrade) new Trade(stop, target, entry, target, openTime, candle.Date,
                                target - entry));
                        }
                    }
                    else
                    {
                        // See above comment on opposite direction trade
                        if (candle.Open >= entry && !executed)
                        {
                            entry = candle.Open;
                            stop = entry + stopSize;
                            executed = true;
                            openTime = candle.Date;
                            continue;
                        }

                        if (candle.High > entry && !executed)
                        {
                            executed = true;
                            openTime = candle.Date;
                            continue;
                        }

                        if (candle.High > stop && executed)
                        {
                            return Option.Some((ITrade) new Trade(stop, target, entry, stop, openTime, candle.Date,
                                -(stop - entry)));
                        }

                        if (candle.Low < target && executed)
                        {
                            return Option.Some((ITrade) new Trade(stop, target, entry, target, openTime, candle.Date,
                                entry - target));
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
            var close = lastCandle.Close;

            return target > stop
                ? Option.Some((ITrade) new Trade(stop, target, entry, close, openTime, lastCandle.Date,
                    close - entry))
                : Option.Some((ITrade) new Trade(stop, target, entry, close, openTime, lastCandle.Date,
                    entry - close));
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

        internal static double GetGapTradeStopLevel(double gap, double entry, double stopSize)
        {
            var stop = gap > 0
                ? entry + stopSize
                : entry - stopSize;

            return stop;
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
