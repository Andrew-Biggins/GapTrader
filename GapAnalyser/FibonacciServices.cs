using System;
using System.Collections.Generic;
using System.Text;

namespace GapAnalyser
{
    public enum FibonacciLevel
    {
        FivePointNine = 59,
        ElevenPointFour = 114,
        TwentyThreePointSix = 236,
        ThirtyEightPointTwo = 382,
        Fifty = 500,
        SixtyOnePointEight = 618,
        SeventyEightPointSix = 786,
        EightyEightPointSix = 886,
        NinetyFourPointOne = 941,
        OneHundred = 1000,
        OneHundredAndTwentySevenPointOne = 1271,
        OneHundredAndFortyOnePointFour = 1414,
        OneHundredAndSixtyOnePointEight = 1618,
        TwoHundred = 2000,
        TwoHundredAndTwentySevenPointOne = 2271,
        TwoHundredAndFortyOnePointFour = 2414,
        TwoHundredAndSixtyOnePointEight = 2618,
        ThreeHundred = 3000,
        ThreeHundredAndSixtyOnePointEight = 3618
    }

    public static class FibonacciServices
    {
        public static Dictionary<FibonacciLevel, FibLevel> NewFibRetraceDictionary()
        {
            var dictionary = new Dictionary<FibonacciLevel, FibLevel>();

            var retraces  = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            for (var i = 0; i < 10; i++)
            {
                dictionary.Add(retraces[i], new FibLevel((double)retraces[i]));
            }

            return dictionary;
        }
    }
}
