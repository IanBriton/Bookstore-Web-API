using BookstoreAPI.Models;

namespace BookstoreAPI.Interface
{
    public interface IAuthorRepository
    {
        ICollection<Author> GetAuthors();
        public Author GetAuthor(int authorId);
        bool AuthorExists(int authorId);
        bool CreateAuthor(Author author);
        bool DeleteAuthor(Author author);
        bool Save();
    }
}
