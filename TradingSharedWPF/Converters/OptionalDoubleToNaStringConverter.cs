using System;
using System.Globalization;
using System.Windows.Data;
using TradingSharedCore.Optional;

namespace TradingSharedWPF.Converters
{
    public sealed class OptionalDoubleToNaStringConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return string.Empty;
            }

            object y = string.Empty;

            var d = (Optional<double>)value;

            d.IfExistsThen(x => { y = x; }).IfEmpty(() => y = "N/A");

            return y;
        }
    }
}
