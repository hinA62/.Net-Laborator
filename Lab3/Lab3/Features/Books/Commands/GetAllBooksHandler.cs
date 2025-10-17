using Lab3.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lab3.Features.Books;

public class GetAllBooksHandler(BooksManagementContext context)
{
    private readonly BooksManagementContext _context = context;
    
    public async Task<IResult> Handle()
    {
        var books = await _context.Books.ToListAsync();
        return Results.Ok(books);
    }
}