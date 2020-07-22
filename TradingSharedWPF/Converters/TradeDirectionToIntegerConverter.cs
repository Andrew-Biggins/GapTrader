using System;
using System.Globalization;
using System.Windows.Data;
using TradingSharedCore;

namespace TradingSharedWPF.Converters
{
    public sealed class TradeDirectionToIntegerConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return -1;
            }

            return (TradeDirection)value switch
            {
                TradeDirection.Long => 0,
                TradeDirection.Short => 1,
                TradeDirection.Both => 2,
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
                -1 => TradeDirection.Long,
                0 => TradeDirection.Long,
                1 => TradeDirection.Short,
                2 => TradeDirection.Both,
                _ => throw new ArgumentOutOfRangeException(value.ToString()),
            };
        }
    }
}
