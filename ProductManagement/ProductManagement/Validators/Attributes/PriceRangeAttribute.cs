using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ProductManagement.Validators.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class PriceRangeAttribute(double min, double max) : ValidationAttribute
{
    private readonly decimal _min = Convert.ToDecimal(min);
    private readonly decimal _max = Convert.ToDecimal(max);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (!decimal.TryParse(value.ToString(), out decimal price))
            return new ValidationResult("Invalid price format.");

        if (price < _min || price > _max)
        {
            string minStr = _min.ToString("C2", CultureInfo.InvariantCulture);
            string maxStr = _max.ToString("C2", CultureInfo.InvariantCulture);
            return new ValidationResult($"Price must be between {minStr} and {maxStr}.");
        }

        return ValidationResult.Success;
    }
}