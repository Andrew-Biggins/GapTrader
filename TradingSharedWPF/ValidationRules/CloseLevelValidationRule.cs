using System.Globalization;
using System.Windows.Controls;
using TradingSharedWPF.Helpers;

namespace TradingSharedWPF.ValidationRules
{
    public sealed class CloseLevelValidationRule : ValidationRule
    {
        public ValidationWrapper CanBeNegative { private get; set; } = new ValidationWrapper();

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.ValidResult;
            }

            // Set default value 
            if (CanBeNegative.Data == null)
            {
                CanBeNegative.Data = false;
            }

            if (double.TryParse((string)value, out var number))
            {
                if (!(bool)CanBeNegative.Data && number < 0)
                {
                    return new ValidationResult(false, "Value must be a non-zero positive");
                }

                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Value must be a number");
        }
    }
}
