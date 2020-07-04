using GapTraderCore.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

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
                case StrategyType.Triangle:
                    return "Triangle";
                case StrategyType.FailedTriangle:
                    return "Failed Triangle";
                case StrategyType.Other:
                    return "Other";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
