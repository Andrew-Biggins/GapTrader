using System.Globalization;
using System.Windows.Controls;

namespace GapTraderWPF.ValidationRules
{
    public class MinimumGapSizeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Value cannot be empty");
            }

            if (double.TryParse((string)value, out var number))
            {
                if (number < 0)
                {
                    return new ValidationResult(false, "Value must be positive");
                }

                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Value must be a number");
        }
    }
}