using Lab3.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Features.Books.Queries;

public class SortBooksHandle(BooksManagementContext context)
{
    private readonly BooksManagementContext _context = context;
    
    public async Task<IEnumerable<Book>> Handle(SortBooksRequest request)
    {
        IQueryable<Book> query = _context.Books;

        if (request.SortByTitle())
        {
            query = request.SortByTitle()
                ? query.OrderBy(b => b.Title)
                : query.OrderByDescending(b => b.Title);
        }
        else
        {
            query = request.SortByYear()
                    ? query.OrderBy(b => b.Year)
                    : query.OrderByDescending(b => b.Year);
        }

        var books = await query.ToListAsync();
        return books;
    }
}