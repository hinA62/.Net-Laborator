using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Features.Product;
using ProductManagement.Persistence;
using ProductManagement.Validators;

var builder = WebApplication.CreateBuilder(args);   

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ProductsManagementContext>(options =>
    options.UseSqlite("Data Source=product_management.db"));
builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductProfileValidator>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductsManagementContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI
    (
        c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API V1");
            c.RoutePrefix = string.Empty;
            c.DisplayRequestDuration();
        }
    );
    app.MapOpenApi();
}

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc
    (
        "v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Product Management API",
            Version = "v1",
            Description = "An API to manage products",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Api Support",
                Email = "suport@exampple.com"
            }
        }
    );
}); 

app.UseHttpsRedirection();

app.MapPost("/users", async (CreateProductProfileRequest request, CreateProductHandler handler)
    => await handler.Handle(request));


app.Run();
