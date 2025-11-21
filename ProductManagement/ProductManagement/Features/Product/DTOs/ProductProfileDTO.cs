namespace ProductManagement.Features.Product;

public class ProductProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string SKU { get; set; }
    public ProductCategory Category { get; set; }
    public decimal Price { get; set; }
    public string FormatPrice() => Price.ToString("C");
    public DateTime ReleaseDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }
    
    public string CategoryDisplay { get; set; }
    public string PriceFormatted { get; set; }
    public string ProductAge { get; set; }
    public string BrandInitials { get; set; }
    public string AvailabilityStatus { get; set; }
}