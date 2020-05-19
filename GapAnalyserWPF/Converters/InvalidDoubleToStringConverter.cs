using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using GapAnalyser;

namespace GapAnalyserWPF.Converters
{
    class InvalidDoubleToStringConverter : MarkupConverter, IValueConverter
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
