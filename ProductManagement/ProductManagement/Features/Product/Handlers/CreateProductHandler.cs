using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductManagement.Logging;
using ProductManagement.Persistence;
using ProductManagement.Validators;
using ProductManagement.Exceptions;


namespace ProductManagement.Features.Product.Handlers;

public class CreateProductHandler
{
    private readonly ProductsManagementContext _context;
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly ILogger<CreateProductProfileValidator> _validatorLogger;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public CreateProductHandler(ProductsManagementContext context, ILogger<CreateProductHandler> logger,
        ILogger<CreateProductProfileValidator> validatorLogger, IMapper mapper, IMemoryCache cache)
    {
        _context = context;
        _logger = logger;
        _validatorLogger = validatorLogger;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IResult> Handle(CreateProductProfileRequest request)
    {
        _logger.Log(
            LogLevel.Information,
            new EventId(ProductLogEvents.ProductCreationStarted, nameof(ProductLogEvents.ProductCreationStarted)),
            "Starting product creation for {Name}", request.Name);

        var operationId = Guid.NewGuid().ToString("N")[..8];
        var stopwatch = Stopwatch.StartNew();
        var validationWatch = new Stopwatch();
        var dbWatch = new Stopwatch();

        validationWatch.Start();
        var validator = new CreateProductProfileValidator(_context, _validatorLogger);
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.Log(
                LogLevel.Warning,
                new EventId(ProductLogEvents.ProductValidationFailed, nameof(ProductLogEvents.ProductValidationFailed)),
                "Validation failed for product {Name}", request.Name);
            throw new ValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
        }
        validationWatch.Stop();

        var skuExists = await _context.Products
            .AnyAsync(p => p.SKU == request.SKU);
        if (skuExists)
        {
            _logger.LogWarning("SKU {SKU} already exists. Product creation aborted.", request.SKU);
            _logger.Log(
                LogLevel.Warning,
                new EventId(ProductLogEvents.SKUValidationPerformed, nameof(ProductLogEvents.SKUValidationPerformed)),
                "Duplicate SKU detected: {SKU}", request.SKU);
            throw new ValidationException($"SKU '{request.SKU}' already exists");
        }


        var product = _mapper.Map<Product>(request);

        dbWatch.Start();
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Product with ID {ProductId} created successfully.", product.Id);
        dbWatch.Stop();

        var dto = null as ProductProfileDto;
        try
        {
            dto = _mapper.Map<ProductProfileDto>(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AutoMapper mapping Product -> ProductProfileDto failed");
            throw new InvalidOperationException($"Mapping Product -> ProductProfileDto failed: {ex.Message}", ex);
        }

        _cache.Remove("all_products");
        _logger.LogInformation("Cache cleared successfully.");
        stopwatch.Stop();

        _logger.LogProductCreationMetrics(
            new ProductCreationMetrics(
                OperationId: operationId,
                ProductName: request.Name,
                SKU: request.SKU,
                Category: request.Category,
                ValidationDuration: validationWatch.Elapsed,
                DatabaseSaveDuration: dbWatch.Elapsed,
                TotalDuration: stopwatch.Elapsed,
                Success: true
            )
        );
        return Results.Created($"/products/{product.Id}", dto);
    }
}