using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using GapTraderCore;
using GapTraderCore.ViewModels;

namespace GapTraderWPF.Converters
{
    public sealed class StrategyTypeToStringConverter : MarkupConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((StrategyType)value)
            {
                case StrategyType.IntoGap:
                    return "Into Gap";
                case StrategyType.OutOfGap:
                    return "Out Of Gap";
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
