using BookstoreAPI.Models;

namespace BookstoreAPI.Interface
{
    public interface IGenreRepository
    {
        ICollection<Genre> GetGenres();
        public Genre GetGenre(int genreId);
        bool CreateGenre(Genre genre);
        bool GenreExists(int genreId);
        bool DeleteGenre(Genre genre);
        bool Save();
    }
}
