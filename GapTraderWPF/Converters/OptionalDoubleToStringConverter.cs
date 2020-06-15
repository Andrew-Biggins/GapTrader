using Foundations.Optional;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GapTraderWPF.Converters
{
    public sealed class OptionalDoubleToStringConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return string.Empty;
            }

            object y = string.Empty;

            var d = (Optional<double>) value;

            d.IfExistsThen(x => { y = x; });

            return y;
        }

        public new object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType is null)
            {
                return null;
            }

            if (double.TryParse((string)value, out double d))
            {
                return Option.Some(d);
            }

            return Option.None<double>();
        }
    }
}
