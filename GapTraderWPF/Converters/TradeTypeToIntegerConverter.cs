using System;
using System.Globalization;
using System.Windows.Data;
using GapTraderCore.ViewModels;

namespace GapTraderWPF.Converters
{
    public sealed class TradeTypeToIntegerConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
            {
                return -1;
            }

            return (TradeType)value switch
            {
                TradeType.IntoGap => 0,
                TradeType.OutOfGap => 1,
                TradeType.Both => 2,
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
                -1 => TradeType.IntoGap,
                0 => TradeType.IntoGap,
                1 => TradeType.OutOfGap,
                2 => TradeType.Both,
                _ => throw new ArgumentOutOfRangeException(value.ToString()),
            };
        }
    }
}
