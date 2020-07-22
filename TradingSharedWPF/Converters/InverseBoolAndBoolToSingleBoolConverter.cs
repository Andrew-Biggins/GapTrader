using System;
using System.Globalization;
using System.Windows.Data;

namespace TradingSharedWPF.Converters
{
    public sealed class InverseBoolAndBoolToSingleBoolConverter : MarkupConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)values[0] && !(bool)values[1])
            {
                return true;
            }

            return false;
        }
    }
}
