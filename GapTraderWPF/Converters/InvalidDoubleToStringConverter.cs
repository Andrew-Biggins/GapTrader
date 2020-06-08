using System;
using System.Globalization;
using System.Windows.Data;

namespace GapTraderWPF.Converters
{
    public class InvalidDoubleToStringConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.IsNaN((double)value) || double.IsInfinity((double)value))
            {
                return "---";
            }

            return value;
        }
    }
}
