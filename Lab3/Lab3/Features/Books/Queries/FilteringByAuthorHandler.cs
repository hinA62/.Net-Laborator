using Lab3.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Features.Books.Queries;

public class FilteringByAuthorHandler(BooksManagementContext context)
{
    private readonly BooksManagementContext _context = context;
    
    public async Task<IEnumerable<Book>> Handle(FilteringByAuthorRequest request)
    {
        var books = await _context.Books
            .Where(b => b.Author.Contains(request.Author))
            .ToListAsync();

        return books;
    }
}