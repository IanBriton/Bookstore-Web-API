using AutoMapper;
using BookstoreAPI.Dto;
using BookstoreAPI.Interface;
using BookstoreAPI.Models;
using BookstoreAPI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorController(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Author>))]
        public IActionResult GetAuthors()
        {
            var authors = _mapper.Map<List<AuthorDto>>(_authorRepository.GetAuthors());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(authors);
        }

        [HttpGet]
        [Route("{authorId}/author")]
        [ProducesResponseType(200, Type = typeof(Author))]
        public IActionResult GetAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var author = _mapper.Map<AuthorDto>(_authorRepository.GetAuthor(authorId));

            if (!ModelState.IsValid) return BadRequest();

            return Ok(author);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult CreateAuthor(AuthorDto authorCreate)
        {
            if (authorCreate == null)
                return BadRequest(ModelState);

            var authors = _authorRepository.GetAuthors().FirstOrDefault(
                a => a.LastName.Trim().ToUpper() == authorCreate.LastName.TrimEnd().ToUpper());
            if(authors != null)
            {
                ModelState.AddModelError("", "Author already exists");
                return StatusCode(422, ModelState);
            }    

            if(!ModelState.IsValid) return BadRequest(ModelState);

            var authorMap = _mapper.Map<Author>(authorCreate);

            if (!_authorRepository.CreateAuthor(authorMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the author.");
                return StatusCode(500, ModelState);
            }
            return Ok("Author successfully created...");
        }
    }
}
