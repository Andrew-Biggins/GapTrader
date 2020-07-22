using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TradingSharedWPF.Converters
{
    public sealed class InverseBoolToHiddenVisibilityConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? Visibility.Hidden : Visibility.Visible;
    }
}
