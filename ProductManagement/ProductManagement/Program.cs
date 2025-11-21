using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Features.Product;
using ProductManagement.Features.Product.Handlers;
using ProductManagement.Mapper;
using ProductManagement.Persistence;
using ProductManagement.Validators;

var builder = WebApplication.CreateBuilder(args);

// DATABASE
builder.Services.AddDbContext<ProductsManagementContext>(options =>
    options.UseSqlite("Data Source=product_management.db"));

// VALIDATION
builder.Services.AddScoped<CreateProductProfileValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductProfileValidator>();

// AUTOMAPPER
builder.Services.AddAutoMapper(typeof(ProductMappingProfile), typeof(AdvancedProductMappingProfile));

// CACHE
builder.Services.AddMemoryCache();

// SWAGGER / OPENAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Product Management API",
        Version = "v1",
        Description = "An API to manage products",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        }
    });
});

// APPLICATION SERVICES
builder.Services.AddScoped<CreateProductHandler>();

var app = builder.Build();

// DATABASE ENSURE CREATED
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductsManagementContext>();
    context.Database.EnsureCreated();
}

// ENDPOINTS
app.MapPost("/products",
    async (CreateProductProfileRequest request, CreateProductHandler handler) =>
    {
        return await handler.Handle(request);
    })
    .WithName("CreateProduct")
    .WithTags("Products")
    .WithOpenApi(op =>
    {
        op.Summary = "Create a new product";
        op.Description = "Creates a new product with validation, mapping, logging, and caching.";
        return op;
    });

app.Run();
