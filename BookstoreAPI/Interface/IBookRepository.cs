using BookstoreAPI.Models;

namespace BookstoreAPI.Interface
{
    public interface IBookRepository
    {
        ICollection<Book> GetBooks();
        public Book GetBook(int bookId);
        bool CreateBook(int genreId, int authorId, Book book);
        bool BookExists(int bookId);
        bool DeleteBook(Book book);
        bool Save();
    }
}
