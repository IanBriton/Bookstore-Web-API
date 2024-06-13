using AutoMapper;
using BookstoreAPI.Dto;
using BookstoreAPI.Dto.OtherObjects;
using BookstoreAPI.Interface;
using BookstoreAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookController(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        [Authorize(Roles = StaticUserRoles.USER)]

        public IActionResult GetBooks()
        {
            var books = _mapper.Map<List<BookDto>>(_bookRepository.GetBooks());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(books);
        }

        [HttpGet]
        [Route("{bookId}/book")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [Authorize(Roles = StaticUserRoles.USER)]

        public IActionResult GetBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var book = _mapper.Map<BookDto>(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(book);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize(Roles = StaticUserRoles.USER)]

        public IActionResult CreateBook([FromQuery] int authorId, [FromQuery] int genreId, [FromBody] BookDto bookCreate)
        {
            if (bookCreate == null)
                return BadRequest(ModelState);

            var books = _bookRepository.GetBooks().FirstOrDefault(b => b.Title.Trim().ToUpper() == bookCreate.Title.TrimEnd().ToUpper());
            if (books != null)
            {
                ModelState.AddModelError("", "Book with the same title already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookMap = _mapper.Map<Book>(bookCreate);
            if (!_bookRepository.CreateBook(authorId, genreId, bookMap))
            {
                ModelState.AddModelError("", "Something went wrong trying to create the book");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created😁😁");
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize(Roles = StaticUserRoles.ADMIN)]
        public IActionResult DeleteBook(int bookId)
        {
            if(!_bookRepository.BookExists(bookId))
                return NotFound();

            var bookToDelete = _bookRepository.GetBook(bookId);

            if (!_bookRepository.DeleteBook(bookToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while trying to delete the book.");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


    }
}
