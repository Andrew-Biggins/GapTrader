using System;
using System.Windows.Data;
using System.Windows.Media;

namespace GapTraderWPF.Converters
{
    public sealed class StringToBrushConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var x = (string)value;
            if (x == string.Empty)
            {
                return Brushes.LawnGreen;
            }
            return x[0].ToString() != "-" ? Brushes.LawnGreen : Brushes.OrangeRed;
        }

        public new object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
