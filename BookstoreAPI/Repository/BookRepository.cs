using BookstoreAPI.Data;
using BookstoreAPI.Interface;
using BookstoreAPI.Models;

namespace BookstoreAPI.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;

        public BookRepository(DataContext context)
        {
            _context = context;
        }
        public bool BookExists(int bookId)
        {
            return _context.Books.Any(b => b.Id == bookId);
        }

        public bool CreateBook(int genreId, int authorId, Book book)
        {
            var authorEntity = _context.Authors.FirstOrDefault(a => a.Id == authorId);
            var genreEntity = _context.Genres.FirstOrDefault(g => g.Id == genreId);

            if (authorEntity == null || genreEntity == null)
            {
                return false;
            }

            //var newBook = new Book
            //{
            //    Title = book.Title,
            //    GenreId = genreId,
            //    AuthorId = authorId,
            //    Author = authorEntity,
            //    Genre = genreEntity
            //};

            book.Author = authorEntity;
            book.Genre = genreEntity;

            _context.Books.Add(book);

            return Save();
        }

        public bool DeleteBook(Book book)
        {
            _context.Remove(book);
            return Save();
        }

        public Book GetBook(int bookId) => _context.Books.FirstOrDefault(b => b.Id == bookId);

        public ICollection<Book> GetBooks()
        {
            return _context.Books.ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
