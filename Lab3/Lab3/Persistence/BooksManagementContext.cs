using Lab3.Features;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Persistence;

public class BooksManagementContext(DbContextOptions<BooksManagementContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
}