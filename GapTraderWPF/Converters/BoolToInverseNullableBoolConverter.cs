using System;
using System.Windows.Data;

namespace GapTraderWPF.Converters
{
    [ValueConversion(typeof(bool), typeof(bool?))]
    public sealed class BoolToInverseNullableBoolConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool?))
                throw new InvalidOperationException("The target must be a nullable boolean");

            return !(bool?)value;
        }

        public new object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }
    }
}
