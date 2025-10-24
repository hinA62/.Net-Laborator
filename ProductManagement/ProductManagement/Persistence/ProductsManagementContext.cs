using Microsoft.EntityFrameworkCore;
using ProductManagement.Features.Product;

namespace ProductManagement.Persistence;

public class ProductsManagementContext (DbContextOptions<ProductsManagementContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}