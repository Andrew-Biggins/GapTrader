using System;
using System.Globalization;
using System.Windows.Data;
using GapTraderCore;

namespace GapTraderWPF.Converters
{
    public class IntegerToFibRetracementConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return -1;
            }

            return (FibonacciLevel)value switch
            {
                FibonacciLevel.FivePointNine => 0,
                FibonacciLevel.ElevenPointFour => 1,
                FibonacciLevel.TwentyThreePointSix => 2,
                FibonacciLevel.ThirtyEightPointTwo => 3,
                FibonacciLevel.Fifty => 4,
                FibonacciLevel.SixtyOnePointEight => 5,
                FibonacciLevel.SeventyEightPointSix => 6,
                FibonacciLevel.EightyEightPointSix => 7,
                FibonacciLevel.NinetyFourPointOne => 8,
                FibonacciLevel.OneHundred => 9,
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
                -1 => FibonacciLevel.FivePointNine,
                0 => FibonacciLevel.FivePointNine,
                1 => FibonacciLevel.ElevenPointFour,
                2 => FibonacciLevel.TwentyThreePointSix,
                3 => FibonacciLevel.ThirtyEightPointTwo,
                4 => FibonacciLevel.Fifty,
                5 => FibonacciLevel.SixtyOnePointEight,
                6 => FibonacciLevel.SeventyEightPointSix,
                7 => FibonacciLevel.EightyEightPointSix,
                8 => FibonacciLevel.NinetyFourPointOne,
                9 => FibonacciLevel.OneHundred,
                _ => throw new ArgumentOutOfRangeException(value.ToString()),
            };
        }
    }
}
