using AutoMapper;
using BookstoreAPI.Dto;
using BookstoreAPI.Interface;
using BookstoreAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreController(IGenreRepository genreRepository, IMapper mapper)
        {
            _genreRepository = genreRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Genre>))]
        public IActionResult GetGenres()
        {
            var genres = _mapper.Map<List<GenreDto>>(_genreRepository.GetGenres());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(genres);
        }

        [HttpGet]
        [Route("{genreId}/genre")]
        [ProducesResponseType(200, Type = typeof(Genre))]
        public IActionResult GetGenre(int genreId)
        {
            if (!_genreRepository.GenreExists(genreId))
                return NotFound();

            var genre = _mapper.Map<GenreDto>(_genreRepository.GetGenre(genreId));

            if (!ModelState.IsValid) return BadRequest();

            return Ok(genre);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult CreateAuthor(GenreDto genreCreate)
        {
            if (genreCreate == null)
                return BadRequest(ModelState);

            var genres = _genreRepository.GetGenres().FirstOrDefault(
                a => a.Name.Trim().ToUpper() == genreCreate.Name.TrimEnd().ToUpper());
            if (genres != null)
            {
                ModelState.AddModelError("", "Author already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var genreMap = _mapper.Map<Genre>(genreCreate);

            if (!_genreRepository.CreateGenre(genreMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the author.");
                return StatusCode(500, ModelState);
            }
            return Ok("Genre successfully created...");
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteGenre(int genreId)
        {
            if (!_genreRepository.GenreExists(genreId))
                return NotFound();

            var genreToDelete = _genreRepository.GetGenre(genreId);

            if (!_genreRepository.DeleteGenre(genreToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting the genre.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
