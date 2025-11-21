namespace ProductManagement.Features.Product;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }
    public string SKU { get; set; }
    public ProductCategory Category { get; set; }
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Product() { }

    public Product(Guid id, string name, string brand, string sku, ProductCategory category, decimal price, DateTime releaseDate, string? imageUrl, bool isAvailable, int stockQuantity, DateTime? updatedAt)
    {
        Id = id;
        Name = name;
        Brand = brand;
        SKU = sku;
        Category = category;
        Price = price;
        ReleaseDate = releaseDate;
        ImageUrl = imageUrl;
        IsAvailable = isAvailable;
        StockQuantity = stockQuantity;
        UpdatedAt = updatedAt;
    }
}
