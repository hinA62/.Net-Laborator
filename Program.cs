using Lab2;

class LibraryApp
{
    public static void Main()
    {
        Borrower borrower1 = new Borrower(1, "John", 
            new List<Book> { new Book("1984", 
                "George Orwell", 1949) });

        Borrower borrower2 = borrower1 with { BorrowedBooks = borrower1
            .BorrowedBooks.Append(new Book("Brave New World",
                "Aldous Huxley", 1932)).ToList() };
        
        BookManager bookManager = new BookManager();
        bookManager.DisplayLibraryBooks();
        bookManager.AddBookByTitle(borrower1);
        bookManager.WhatIs(borrower2);
        bookManager.WhatIs(Book.libraryBooks.First());
        bookManager.WhatIs("dsb");
        var recentBooks = bookManager.BooksAfter2010();
        Console.WriteLine("Books published after 2010:");
    }
}
