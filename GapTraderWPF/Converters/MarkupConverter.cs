using System;
using System.Globalization;
using System.Windows.Markup;

namespace GapTraderWPF.Converters
{
    public abstract class MarkupConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider) => this;

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            HandleConvertBackNotSupported();

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            HandleConvertBackNotSupported();

        private object[] HandleConvertBackNotSupported() =>
            throw new NotSupportedException($"{nameof(ConvertBack)} is not supported on {GetType()}.");
    }
}
