using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProductManagement.Validators.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ValidSkuAttribute : ValidationAttribute, IClientModelValidator
{
    public ValidSkuAttribute()
    {
        ErrorMessage = "SKU must be alphanumeric, 5-20 characters, and can include hyphens.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) 
            return ValidationResult.Success;

        var sku = value.ToString()!.Replace(" ", ""); // remove spaces

        if (sku.Length < 5 || sku.Length > 20 || !Regex.IsMatch(sku, @"^[A-Z0-9\-]+$", RegexOptions.IgnoreCase))
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-validsku", ErrorMessage);
    }

    private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (attributes.ContainsKey(key)) return false;
        attributes.Add(key, value);
        return true;
    }
}