using System;
using System.Collections.Generic;

namespace TradingSharedCore
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
        public static int FirstFibRetraceIndex = 0;
        public static int LastFibRetraceIndex = 9;
        public static int FirstFibExtensionIndex = 10;
        public static int LastFibExtensionIndex = 18;

        public static Dictionary<FibonacciLevel, FibLevel> NewFibRetraceDictionary()
        {
            var dictionary = new Dictionary<FibonacciLevel, FibLevel>();

            var retraces  = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            for (var i = FirstFibRetraceIndex; i <= LastFibRetraceIndex; i++)
            {
                dictionary.Add(retraces[i], new FibLevel((double)retraces[i]));
            }

            return dictionary;
        }

        public static Dictionary<FibonacciLevel, FibLevel> NewFibExtensionDictionary()
        {
            var dictionary = new Dictionary<FibonacciLevel, FibLevel>();

            var extensions = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            for (var i = FirstFibExtensionIndex; i <= LastFibExtensionIndex; i++)
            {
                dictionary.Add(extensions[i], new FibLevel((double)extensions[i]));
            }

            return dictionary;
        }

        public static List<FibonacciLevel> GetFibRetraceLevels()
        {
            var retraces = new List<FibonacciLevel>();

            var fibs = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            for (var i = FirstFibRetraceIndex; i <= LastFibRetraceIndex; i++)
            {
                retraces.Add(fibs[i]);
            }

            return retraces;
        }

        public static List<FibonacciLevel> GetFibExtensionLevels()
        {
            var extensions = new List<FibonacciLevel>();

            var fibs = (FibonacciLevel[])Enum.GetValues(typeof(FibonacciLevel));

            for (var i = FirstFibExtensionIndex; i <= LastFibExtensionIndex; i++)
            {
                extensions.Add(fibs[i]);
            }

            return extensions;
        }
    }
}
