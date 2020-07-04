using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace GapTraderWPF.Converters
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
