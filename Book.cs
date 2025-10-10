namespace Lab2;

public record Book(string Title, string Author, int YearPublished)
{
    public static List<Book> libraryBooks =
    [
        new Book("The Great Gatsby", "F. Scott Fitzgerald", 1925),
        new Book("To Kill a Mockingbird", "Harper Lee", 1960),
        new Book("1984", "George Orwell", 1949),
        new Book("Pride and Prejudice", "Jane Austen", 1813),
        new Book("The Catcher in the Rye", "J.D. Salinger", 1951),
        new Book("sgerg", "knvwl", 2014),
        new Book("The Fault in Our Stars", "John Green", 2012),
        new Book("Ready Player One", "Ernest Cline", 2010),
    ];
};

