using Lab2;

namespace Lab2
{
    public record Borrower(int Id, string Name, List<Book> BorrowedBooks);
}

public class BookManager
{
    private List<Book> myBooks = new List<Book>();

    public void AddBookByTitle()
    {
        Console.Write("Enter the book title: ");
        string title = Console.ReadLine() ?? "";

        //search the title in libraryBooks
        Book? book = Book.libraryBooks.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (book != null)
        {
            myBooks.Add(book);
            Console.WriteLine($"Book '{book.Title}' added to your collection.");
        }
    }
    
    public void DisplayLibraryBooks()
    {
        Console.WriteLine("Available books in the library:");
        foreach (var book in Book.libraryBooks)
        {
            Console.WriteLine($"- {book.Title} by {book.Author} ({book.YearPublished})");
        }
    }

    public void WhatIs(Object obj)
    {
        if (obj is Book book)
        {
            Console.WriteLine("Book Title: "
                              + book.Title 
                              + "Year: " 
                              + book.YearPublished);
        }
        
        if (obj is Borrower borrower)
        {
            Console.WriteLine("Borrower Name: " 
                              + borrower.Name 
                              + "Number of borrowed books: " 
                              + borrower.BorrowedBooks.Count);
        }
        
        else 
        {
            Console.WriteLine("Unknown type");
        }
    }
    
    public List<Book> BooksAfter2010()
    {
        return Book.libraryBooks.Where(b => b.YearPublished > 2010).ToList();
    }
}