using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using Foundations.Optional;

namespace GapTraderWPF.Converters
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
