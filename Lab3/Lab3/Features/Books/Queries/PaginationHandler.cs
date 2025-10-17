using Lab3.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Features.Books.Queries;

public class PaginationHandler(BooksManagementContext context)
{
    private readonly BooksManagementContext _context = context;
    
    public async Task<IEnumerable<Book>> Handle(PaginationRequest request)
    {
        IQueryable<Book> query = _context.Books;

        var books = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return books;
    }
}