namespace ProductManagement.Features.Product;

public record Product
    (
        string Name,
        string Brand,
        string SKU,
        ProductCategory Category,
        decimal Price,
        DateTime ReleaseDate,
        string? ImageUrl,
        bool IsAvailable,
        int StockQuantity
    );