using System;
using System.Globalization;
using System.Windows.Data;
using GapTraderCore;

namespace GapTraderWPF.Converters
{
    public sealed class FibLevelToNumericStringConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return null;
            }

            return ((FibonacciLevel)value) switch
            {
                FibonacciLevel.FivePointNine => "5.9%",
                FibonacciLevel.ElevenPointFour => "11.4%",
                FibonacciLevel.TwentyThreePointSix => "23.6%",
                FibonacciLevel.ThirtyEightPointTwo => "38.2%",
                FibonacciLevel.Fifty => "50%",
                FibonacciLevel.SixtyOnePointEight => "61.8%",
                FibonacciLevel.SeventyEightPointSix => "78.6%",
                FibonacciLevel.EightyEightPointSix => "88.6%",
                FibonacciLevel.NinetyFourPointOne => "94.1%",
                FibonacciLevel.OneHundred => "100%",
                FibonacciLevel.OneHundredAndTwentySevenPointOne => "127.1%",
                FibonacciLevel.OneHundredAndFortyOnePointFour => "141.4%",
                FibonacciLevel.OneHundredAndSixtyOnePointEight => "161.8%",
                FibonacciLevel.TwoHundred => "200%",
                FibonacciLevel.TwoHundredAndTwentySevenPointOne => "227.1%",
                FibonacciLevel.TwoHundredAndFortyOnePointFour => "241.4%",
                FibonacciLevel.TwoHundredAndSixtyOnePointEight => "261.8%",
                FibonacciLevel.ThreeHundred => "300%",
                FibonacciLevel.ThreeHundredAndSixtyOnePointEight => "361.8%",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
            };
        }
    }
}
