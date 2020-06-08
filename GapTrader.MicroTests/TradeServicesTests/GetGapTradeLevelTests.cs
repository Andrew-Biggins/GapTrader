using GapAnalyser.MicroTests.Gwt;
using GapTraderCore;
using Xunit;
using static GapAnalyser.TradeServices;

namespace GapAnalyser.MicroTests.TradeServicesTests
{
    public class GetGapTradeLevelTests
    {
        [GwtTheory("Given valid data",
            "when the entry level is requested",
            "the entry level is correct")]
        [InlineData(100, 100, 100, false, FibonacciLevel.FivePointNine, 200)] // Into gap trade - points only entry
        [InlineData(-100, 100, 100, false, FibonacciLevel.FivePointNine, 0)] // Out of gap trade - points only entry
        [InlineData(300, 500, 100, true, FibonacciLevel.OneHundredAndSixtyOnePointEight, 785.4)] // Into gap trade - gap up
        [InlineData(-500, 400, -100, true, FibonacciLevel.TwentyThreePointSix, 382)] // Out of gap trade - gap up
        [InlineData(-202, 24120, -50, true, FibonacciLevel.OneHundredAndTwentySevenPointOne, 24115.26)] // Into gap trade - gap down
        [InlineData(202, 24120, 100, true, FibonacciLevel.TwentyThreePointSix, 24267.67)] // Out of gap trade - gap down
        public void T1(double gap, double open, double pointsEntry, bool fibEntry, FibonacciLevel fibLevel, double expected)
        {
            var actual = GetGapTradeEntryLevel(gap, open, pointsEntry, fibEntry, fibLevel);

            Assert.Equal(expected, actual);
        }

        [GwtTheory("Given valid data",
            "when the stop level is requested",
            "the stop level is correct")]
        [InlineData(-100, 100, 100, 0)] // Into gap trade - gap down
        [InlineData(100, 100, 100, 200)] // Out of gap trade - gap down
        [InlineData(300, 500, 50, 550)] // Into gap trade - gap up
        [InlineData(-500, 400, 50, 350)] // Out of gap trade - gap up
        public void T2(double gap, double open, double stopSize, double expected)
        {
            var actual = GetGapTradeStopLevel(gap, open, stopSize);

            Assert.Equal(expected, actual);
        }

        [GwtTheory("Given valid data",
            "when the target level is requested",
            "the target level is correct")]
        [InlineData(100, 100, 100, 100, false, FibonacciLevel.FivePointNine, 0)] // Into gap trade - points only target
        [InlineData(-100, 100, 100, 100, false, FibonacciLevel.FivePointNine, 200)] // Out of gap trade - points only target
        [InlineData(300, 0, 500, 100, true, FibonacciLevel.Fifty, 250)] // Into gap trade - gap up
        [InlineData(-500, 0, 400, 100, true, FibonacciLevel.TwoHundredAndSixtyOnePointEight, 1309)] // Out of gap trade - gap up
        [InlineData(-202, 0, 24000, 100, true, FibonacciLevel.SeventyEightPointSix, 24258.77)] // Into gap trade - gap down
        [InlineData(202, 0, 24000, 100, true, FibonacciLevel.OneHundredAndFortyOnePointFour, 23816.37)] // Out of gap trade - gap down
        [InlineData(200, 0, 24120, 0, true, FibonacciLevel.OneHundredAndFortyOnePointFour, 24037.2)] 
        public void T3(double gap, double entry, double open, double pointsTarget, bool fibTarget, FibonacciLevel fibLevel, double expected)
        {
            var actual = GetGapTradeTargetLevel(gap, entry, open, pointsTarget, fibTarget, fibLevel);

            Assert.Equal(expected, actual);
        }
    }
}
