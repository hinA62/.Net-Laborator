using Lab2;

namespace Lab2
{
    public record Borrower(int Id, string Name, List<Book> BorrowedBooks)
    {
        public List<Book> myBooks = new List<Book>();
    }
}
