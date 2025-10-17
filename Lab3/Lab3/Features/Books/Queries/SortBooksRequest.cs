using Lab3.Persistence;

namespace Lab3.Features.Books.Queries;

public class SortBooksRequest(string Title, int Year)
{
    public bool SortByTitle()
    {        
        return !string.IsNullOrEmpty(Title);
    }
    
    public bool SortByYear()
    {
        return Year > 0;
    }
}
