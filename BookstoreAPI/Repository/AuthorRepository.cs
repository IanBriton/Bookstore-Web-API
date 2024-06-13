using BookstoreAPI.Data;
using BookstoreAPI.Interface;
using BookstoreAPI.Models;

namespace BookstoreAPI.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly DataContext _context;

        public AuthorRepository(DataContext context)
        {
            _context = context;
        }

        public bool AuthorExists(int authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public bool CreateAuthor(Author author)
        {
            _context.Add(author);
            return Save();
        }

        public bool DeleteAuthor(Author author)
        {
            _context.Remove(author);
            return Save();
        }

        public Author GetAuthor(int authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        public ICollection<Author> GetAuthors()
        {
            return _context.Authors.ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
