using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Library.API.Helpers;
using AutoMapper;
using System;
using Library.API.Entities;
using Microsoft.AspNetCore.Http;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {
            var authors = libraryRepository.GetAuthors();

            var dtos = Mapper.Map<IEnumerable<AuthorDto>>(authors);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthor(Guid id)
        {
            var author = libraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            var dto = Mapper.Map<AuthorDto>(author);

            return Ok(dto);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody]AuthorForCreationDto dto)
        {
            if (dto == null)
                return BadRequest();

            var authorEntity = Mapper.Map<Author>(dto);

            libraryRepository.AddAuthor(authorEntity);

            if (!libraryRepository.Save())
                return StatusCode(500, "A problem happend with handling your request");

            var author = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (libraryRepository.AuthorExists(id))
                return new StatusCodeResult(StatusCodes.Status409Conflict);

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var author = libraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            libraryRepository.DeleteAuthor(author);

            if (!libraryRepository.Save())
                throw new Exception($"Deleting author {id} failed on save");

            return NoContent();
        }
    }
}