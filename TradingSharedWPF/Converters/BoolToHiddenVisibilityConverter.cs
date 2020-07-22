using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TradingSharedWPF.Converters
{
    public class BoolToHiddenVisibilityConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? Visibility.Visible : Visibility.Hidden;
    }
}
