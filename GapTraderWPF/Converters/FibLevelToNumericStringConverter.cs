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
            switch ((FibonacciLevel)value)
            {
                case FibonacciLevel.FivePointNine:
                    return "5.9%";
                case FibonacciLevel.ElevenPointFour:
                    return "11.4%";
                case FibonacciLevel.TwentyThreePointSix:
                    return "23.6%";
                case FibonacciLevel.ThirtyEightPointTwo:
                    return "38.2%";
                case FibonacciLevel.Fifty:
                    return "50%";
                case FibonacciLevel.SixtyOnePointEight:
                    return "61.8%";
                case FibonacciLevel.SeventyEightPointSix:
                    return "78.6%";
                case FibonacciLevel.EightyEightPointSix:
                    return "88.6%";
                case FibonacciLevel.NinetyFourPointOne:
                    return "94.1%";
                case FibonacciLevel.OneHundred:
                    return "100%";
                case FibonacciLevel.OneHundredAndTwentySevenPointOne:
                    return "127.1%";
                case FibonacciLevel.OneHundredAndFortyOnePointFour:
                    return "141.4%";
                case FibonacciLevel.OneHundredAndSixtyOnePointEight:
                    return "161.8%";
                case FibonacciLevel.TwoHundred:
                    return "200%";
                case FibonacciLevel.TwoHundredAndTwentySevenPointOne:
                    return "227.1%";
                case FibonacciLevel.TwoHundredAndFortyOnePointFour:
                    return "241.4%";
                case FibonacciLevel.TwoHundredAndSixtyOnePointEight:
                    return "261.8%";
                case FibonacciLevel.ThreeHundred:
                    return "300%";
                case FibonacciLevel.ThreeHundredAndSixtyOnePointEight:
                    return "361.8%";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
