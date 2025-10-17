using Lab3.Persistence;

namespace Lab3.Features.Books;

public class DeleteBookHandler(BooksManagementContext context)
{
    public async Task<IResult> Handle(DeleteBookRequest request)
    {
        var book = await context.Books.FindAsync(request.Id);
        if (book == null)
        {
            return Results.NotFound();
        }

        context.Books.Remove(book);
        await context.SaveChangesAsync();

        return Results.NoContent();
    }
}