using System;
using System.Globalization;
using System.Windows.Data;
using TradingSharedCore;

namespace TradingSharedWPF.Converters
{
    public sealed class IntegerToFibExtensionConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return -1;
            }

            return (FibonacciLevel)value switch
            {
                FibonacciLevel.OneHundredAndTwentySevenPointOne => 0,
                FibonacciLevel.OneHundredAndFortyOnePointFour => 1,
                FibonacciLevel.OneHundredAndSixtyOnePointEight => 2,
                FibonacciLevel.TwoHundred => 3,
                FibonacciLevel.TwoHundredAndTwentySevenPointOne => 4,
                FibonacciLevel.TwoHundredAndFortyOnePointFour => 5,
                FibonacciLevel.TwoHundredAndSixtyOnePointEight => 6,
                FibonacciLevel.ThreeHundred => 7,
                FibonacciLevel.ThreeHundredAndSixtyOnePointEight => 8,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public new object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType is null)
            {
                return null;
            }

            return ((int)value) switch
            {
                -1 => FibonacciLevel.OneHundredAndTwentySevenPointOne,
                0 => FibonacciLevel.OneHundredAndTwentySevenPointOne,
                1 => FibonacciLevel.OneHundredAndFortyOnePointFour,
                2 => FibonacciLevel.OneHundredAndSixtyOnePointEight,
                3 => FibonacciLevel.TwoHundred,
                4 => FibonacciLevel.TwoHundredAndTwentySevenPointOne,
                5 => FibonacciLevel.TwoHundredAndFortyOnePointFour,
                6 => FibonacciLevel.TwoHundredAndSixtyOnePointEight,
                7 => FibonacciLevel.ThreeHundred,
                8 => FibonacciLevel.ThreeHundredAndSixtyOnePointEight,
                _ => throw new ArgumentOutOfRangeException(value.ToString()),
            };
        }
    }
}
