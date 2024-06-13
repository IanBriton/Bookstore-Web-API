using BookstoreAPI.Data;
using BookstoreAPI.Interface;
using BookstoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreAPI.Repository
{
    public class GenreRepository : IGenreRepository
    {
        private readonly DataContext _context;

        public GenreRepository(DataContext context)
        {
            _context = context;
        }
        public bool CreateGenre(Genre genre)
        {
            _context.Add(genre);
            return Save();
        }

        public bool DeleteGenre(Genre genre)
        {
            _context.Remove(genre);
            return Save();
        }

        public bool GenreExists(int genreId)
        {
            return _context.Books.Any(g => g.Id == genreId);
        }

        public Genre GetGenre(int genreId)
        {
            return _context.Genres.FirstOrDefault(g => g.Id == genreId);
        }

        public ICollection<Genre> GetGenres()
        {
            return _context.Genres.ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
