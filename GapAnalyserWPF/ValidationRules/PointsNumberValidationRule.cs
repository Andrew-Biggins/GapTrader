using System.Globalization;
using System.Windows.Controls;
using GapAnalyserWPF.Helpers;

namespace GapAnalyserWPF.ValidationRules
{
    public sealed class PointsNumberValidationRule : ValidationRule
    {
        public ValidationWrapper CanBeNegative { private get; set; } = new ValidationWrapper();

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Value cannot be empty");
            }

            // Set default value 
            if (CanBeNegative.Data == null)
            {
                CanBeNegative.Data = false;
            }
            
            if(double.TryParse((string)value, out var number))
            {
                if (!(bool)CanBeNegative.Data && number < 1)
                {
                    return new ValidationResult(false, "Value must be a non-zero positive");
                }

                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Value must be a number");
        }
    }
}
