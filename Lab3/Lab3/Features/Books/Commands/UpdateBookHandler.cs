using Lab3.Persistence;

namespace Lab3.Features.Books;

public class UpdateBookHandler(BooksManagementContext context)
{
    private readonly BooksManagementContext _context = context;
    
    public async Task<IResult> Handle(UpdateBookRequest request)
    {
        var book = await _context.Books.FindAsync(request.Id);
        if (book == null)
        {
            return Results.NotFound();
        }

        book = book with { Title = request.Title };
        book = book with { Author = request.Author };
        book = book with { Year = request.Year };

        await _context.SaveChangesAsync();

        return Results.Ok(book);
    }
}