using System;
using System.Globalization;
using System.Windows.Data;

namespace TradingSharedWPF.Converters
{
    public sealed class IncrementToIntegerConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return -1;
            }

            return value switch
            {
                5 => 0,
                10 => 1,
                20 => 2,
                50 => 3,
                100 => 4,
                200 => 5,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public new object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType is null)
            {
                return null;
            }

            return value switch
            {
                -1 => 5,
                0 => 5,
                1 => 10,
                2 => 20,
                3 => 50,
                4 => 100,
                5 => 200,
                _ => throw new ArgumentOutOfRangeException(value.ToString()),
            };
        }
    }
}
