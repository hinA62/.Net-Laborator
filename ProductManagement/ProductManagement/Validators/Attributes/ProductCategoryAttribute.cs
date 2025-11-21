using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Validators.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ProductCategoryAttribute(params string[] allowedCategories) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success;

        string category = value.ToString()!;

        if (!allowedCategories.Contains(category, StringComparer.OrdinalIgnoreCase))
        {
            string allowed = string.Join(", ", allowedCategories);
            return new ValidationResult($"Invalid category. Allowed categories are: {allowed}.");
        }

        return ValidationResult.Success;
    }
}