namespace ProductManagement.Features.Product;

public class ProductProfileDto
{
    Guid Id { get; set; }
    string Name { get; set; }
    string Brand { get; set; }
    string SKU { get; set; }
    ProductCategory Category { get; set; }
    decimal Price { get; set; }
    string FormatPrice() => Price.ToString("C");
    DateTime ReleaseDate { get; set; }
    DateTime CreatedAt { get; set; }
    string? ImageUrl { get; set; }
    bool IsAvailable { get; set; }
    int StockQuantity { get; set; }
    string ProductAge => (DateTime.Now - ReleaseDate).Days.ToString();
    string BrandInitials => string.Concat(Brand.Split(' ').Select(word => word[0])).ToUpper();
    string AvailabilityStatus => IsAvailable ? "In Stock" : "Out of Stock";
    
}