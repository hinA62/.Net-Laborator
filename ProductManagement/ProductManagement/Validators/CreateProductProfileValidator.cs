using FluentValidation;
using ProductManagement.Features.Product;

namespace ProductManagement.Validators;

public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileRequest>
{
    public CreateProductProfileValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(50).WithMessage("Product name cannot exceed 50 characters.");
        
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(30).WithMessage("Brand cannot exceed 30 characters.");
        
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .Matches(@"^[A-Z0-9\-]+$").WithMessage("SKU must be alphanumeric and can include hyphens.");
        
        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Category must be a valid product category.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        
        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Release date cannot be in the future.");
        
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
    }
}