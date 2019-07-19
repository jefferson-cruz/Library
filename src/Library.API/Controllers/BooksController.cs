using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.API.Entities;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        public IActionResult Index(Guid authorId)
        {
            if (!libraryRepository.AuthorExists(authorId))
                return NotFound();

            var books = libraryRepository.GetBooksForAuthor(authorId);

            var dtos = Mapper.Map<IEnumerable<BookDto>>(books);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!libraryRepository.AuthorExists(authorId))
                return NotFound();

            var book = libraryRepository.GetBookForAuthor(authorId, id);

            if (book == null)
                return NotFound();

            var dto = Mapper.Map<BookDto>(book);

            return Ok(dto);
        }

        [HttpPost]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookForCreationDto dto)
        {
            if (dto == null)
                return BadRequest();

            if (!libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookEntity = Mapper.Map<Book>(dto);

            libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (!libraryRepository.Save())
                throw new Exception($"Creating a book for author {authorId} failed on save");

            var bookToReturn = Mapper.Map<BookDto>(bookEntity);

            return CreatedAtAction(
                nameof(GetBookForAuthor), 
                new { authorId, id = bookToReturn.Id }, 
                bookToReturn
            );
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookFromAuthor = libraryRepository.GetBookForAuthor(authorId, id);

            if (bookFromAuthor == null)
                return NotFound();

            libraryRepository.DeleteBook(bookFromAuthor);

            if (!libraryRepository.Save())
                throw new Exception($"Deleting a book for author {authorId} failed on save");

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookFromAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto dto) 
        {
            if (dto == null)
                return BadRequest();

            if (!libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookFromAuthor = libraryRepository.GetBookForAuthor(authorId, id);

            if (bookFromAuthor == null)
            {
                var bookToAdd = Mapper.Map<Book>(dto);
                bookToAdd.Id = id;

                libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!libraryRepository.Save())
                    throw new Exception($"Upserting book {id} for author {authorId} failed on save");

                var bookToReturn = Mapper.Map<BookDto>(bookToAdd);

                return CreatedAtAction(
                    nameof(GetBookForAuthor),
                    new { authorId, id = bookToReturn.Id },
                    bookToReturn);
            }
            
            Mapper.Map(dto, bookFromAuthor);

            libraryRepository.UpdateBookForAuthor(bookFromAuthor);

            if (!libraryRepository.Save())
                throw new Exception($"Updating a book {id} for author {authorId} failed on save");

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id, [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            if (!libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookFromAuthor = libraryRepository.GetBookForAuthor(authorId, id);

            if (bookFromAuthor == null)
            {
                var book = new BookForUpdateDto();

                patchDoc.ApplyTo(book);

                var bookToAdd = Mapper.Map<Book>(book);
                bookToAdd.Id = id;

                libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!libraryRepository.Save())
                    throw new Exception($"Upserting book {id} for author {authorId} failed on save");

                var bookToReturn = Mapper.Map<BookDto>(bookToAdd);

                return CreatedAtAction(
                    nameof(GetBookForAuthor),
                    new { authorId, id = bookToReturn.Id },
                    bookToReturn);
            }
                //return NotFound();

            var bookToPatch = Mapper.Map<BookForUpdateDto>(bookFromAuthor);

            patchDoc.ApplyTo(bookToPatch);

            //add validation

            Mapper.Map(bookToPatch, bookFromAuthor);

            libraryRepository.UpdateBookForAuthor(bookFromAuthor);

            if (!libraryRepository.Save())
                throw new Exception($"Patching a book {id} for author {authorId} failed on save");

            return NoContent();
        }
    }
}