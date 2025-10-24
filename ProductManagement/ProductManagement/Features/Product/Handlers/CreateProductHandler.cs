using ProductManagement.Persistence;
using ProductManagement.Validators;

namespace ProductManagement.Features.Product;

public class CreateProductHandler (ProductsManagementContext context, ILogger<CreateProductHandler> logger)
{
    private readonly ProductsManagementContext _context = context;

    public async Task<IResult> Handle(CreateProductProfileRequest request)
    {
        logger.LogInformation("Creating a new product with name: {Name} and price: {Price}",
            request.Name, request.Price);

        var validator = new CreateProductProfileValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
            }

            return Results.BadRequest(validationResult.Errors);
        }

        var product = new Product(
            request.Name,
            request.Brand,
            request.SKU,
            request.Category,
            request.Price,
            request.ReleaseDate,
            request.ImageUrl,
            request.IsAvailable,
            request.StockQuantity
        );
        context.Products.Add(product);
        await context.SaveChangesAsync();
        logger.LogInformation("Product created with Name: {ProductName}", product.Name);
        
        return Results.Created($"/products/{product.Name}", product);
    }
}