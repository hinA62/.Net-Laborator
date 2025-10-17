using Lab3.Persistence;

namespace Lab3.Features.Books;

public class CreateBookHandler(BooksManagementContext context)
{
    public async Task<IResult> Handle(CreateBookRequest request)
    {
        var book = new Book(request.Id, request.Title, request.Author, request.Year);
        context.Books.Add(book);
        await context.SaveChangesAsync();

        return Results.Created($"/books/{book.Id}", book);
    }
}