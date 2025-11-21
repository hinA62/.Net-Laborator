using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Features.Product;
using ProductManagement.Persistence;

namespace ProductManagement.Validators;

public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileRequest>
{
    private readonly ProductsManagementContext _db;
    private readonly ILogger<CreateProductProfileValidator> _logger;
    
    private static readonly string[] InappropriateWords = { "not allowed", "badword", "bad word", "notallowed" };
    private static readonly string[] RestrictedHomeWords = { "furniture", "home", "kitchen", "bedroom", "living room" };
    private static readonly string[] TechnologyKeywords = { "AI", "Computer", "Laptop", "Smart", "Phone", "Tech", "Gadget", "Electronics" };
    public CreateProductProfileValidator(ProductsManagementContext db, ILogger<CreateProductProfileValidator> logger)
    {
        this._db = db;
        _logger = logger;
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(50).WithMessage("Product name cannot exceed 50 characters.")
            .Must(BeValidName).WithMessage("Product name contains inappropriate words.")
            .WithMessage("Product name contains inappropriate words.")
            .MustAsync(BeUniqueName).WithMessage("A product with this name already exists for this brand.");
        
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MinimumLength(2).MaximumLength(100).WithMessage("Brand must be between 2 and 100 characters.")
            .Must(BeValidBrandName).WithMessage("Brand name contains inappropriate characters.");
        
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .Must(BeValidSKU).WithMessage("SKU must be alphanumeric, 5-20 chars, hyphens allowed.")
            .MustAsync(BeUniqueSKU).WithMessage("SKU must be unique.");
        
        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Category must be a valid product category.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.")
            .LessThan(10000).WithMessage("Price cannot exceed 10,000.");
        
        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Release date cannot be in the future.")
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Release date cannot be before 1900.");
        
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.")
            .LessThanOrEqualTo(100000).WithMessage("Stock cannot exceed 100,000.");
        
        RuleFor(x => x.ImageUrl)
            .Must(BeValidImageUrl).When(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
            .WithMessage("Image URL must be a valid HTTP/HTTPS URL ending with .jpg/.jpeg/.png/.gif/.webp");

        RuleFor(x => x)
            .MustAsync(PassBusinessRules)
            .WithMessage("Business rules validation failed.");
        
        // CONDITIONAL VALIDATION
        When(x => x.Category == ProductCategory.Electronics, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(50).WithMessage("Electronics price must be at least $50.");

            RuleFor(x => x.Name)
                .Must(ContainTechnologyKeywords)
                .WithMessage("Electronics product name must contain technology keywords.");

            RuleFor(x => x.ReleaseDate)
                .GreaterThan(DateTime.UtcNow.AddYears(-5))
                .WithMessage("Electronics must be released within the last 5 years.");
        });

        When(x => x.Category == ProductCategory.Home, () =>
        {
            RuleFor(x => x.Price)
                .LessThanOrEqualTo(200).WithMessage("Home product price cannot exceed $200.");

            RuleFor(x => x.Name)
                .Must(BeAppropriateForHome)
                .WithMessage("Home product name contains inappropriate content.");
        });

        When(x => x.Category == ProductCategory.Clothing, () =>
        {
            RuleFor(x => x.Brand)
                .MinimumLength(3).WithMessage("Clothing brand must be at least 3 characters.");
        });

        // CROSS-FIELD VALIDATION
        RuleFor(x => x)
            .Must(x => !(x.Price > 100 && x.StockQuantity > 20))
            .WithMessage("Expensive products (>$100) must have limited stock (≤20 units).");

        RuleFor(x => x)
            .Must(x => !(x.Category == ProductCategory.Electronics && x.ReleaseDate < DateTime.UtcNow.AddYears(-5)))
            .WithMessage("Electronics must be released within last 5 years.");

        // BUSINESS RULES ASYNC
        RuleFor(x => x)
            .MustAsync(PassBusinessRules)
            .WithMessage("Business rules validation failed.");
    }
    
    private bool BeValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        return !InappropriateWords
            .Any(w => name.Contains(w, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<bool> BeUniqueName(CreateProductProfileRequest req, string name, CancellationToken ct)
    {
        _logger.LogInformation("Validating unique name for product: {Name}, Brand: {Brand}", req.Name, req.Brand);

        return !await _db.Products
            .AnyAsync(x => x.Name == name && x.Brand == req.Brand, ct);
    }
    
    private bool BeValidBrandName(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            return false;

        return Regex.IsMatch(brand, @"^[A-Za-z0-9\s\-\.'`]+$");
    }

    private bool BeValidSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return false;

        if (sku.Length < 5 || sku.Length > 20)
            return false;

        return Regex.IsMatch(sku, @"^[A-Z0-9\-]+$");
    }

    private async Task<bool> BeUniqueSKU(CreateProductProfileRequest req, string sku, CancellationToken ct)
    {
        _logger.LogInformation("Validating unique SKU: {SKU}", sku);
        
        return !await _db.Products.AnyAsync(x => x.SKU == sku, ct);
    }

    private bool BeValidImageUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true; // optional

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return false;

        string[] allowed = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        return allowed.Any(ext => url.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
    }
    
    private bool ContainTechnologyKeywords(string name)
    {
        return TechnologyKeywords.Any(k => name.Contains(k, StringComparison.OrdinalIgnoreCase));
    }

    private bool BeAppropriateForHome(string name)
    {
        return !RestrictedHomeWords.Any(w => name.Contains(w, StringComparison.OrdinalIgnoreCase));
    }
    
    private async Task<bool> PassBusinessRules(CreateProductProfileRequest req, CancellationToken ct)
    {
        _logger.LogInformation("Running business rule validation for {Name}", req.Name);
        
        // RULE 1: Max 500 products per day
        int productsToday = await _db.Products
            .CountAsync(x => x.ReleaseDate.Date == DateTime.UtcNow.Date, ct);

        if (productsToday >= 500)
        {
            _logger.LogWarning("Business rule failed: daily limit exceeded ({Count})", productsToday);
            return false;
        }

        // RULE 2: Electronics price >= 50
        if (req.Category == ProductCategory.Electronics && req.Price < 50)
        {
            _logger.LogWarning("Business rule failed: electronics minimum price not met (${Price})", req.Price);
            return false;
        }
        // RULE 3: Home category name restrictions
        if (req.Category == ProductCategory.Home &&
            RestrictedHomeWords.Any(w => req.Name.Contains(w, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Business rule failed: restricted home product content");
            return false;
        }
        // RULE 4: Expensive products (>500) must have stock <= 10
        if (req.Price > 500 && req.StockQuantity > 10)
        {
            _logger.LogWarning("Business rule failed: high-value product stock cannot exceed 10.");
            return false;
        }
        return true;
    }
}