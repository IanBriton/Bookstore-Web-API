using AutoMapper;
using BookstoreAPI.Dto;
using BookstoreAPI.Models;

namespace BookstoreAPI.Helper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Book, BookDto>();
            CreateMap<Author, AuthorDto>();
            CreateMap<Genre, GenreDto>();
            CreateMap<BookDto, Book>();
            CreateMap<AuthorDto, Author>();
            CreateMap<GenreDto, Genre>();
        }
    }
}
