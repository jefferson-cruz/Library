using System;
using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/contacts")]
    public class AuthorContactController : Controller
    {
        private readonly ILibraryRepository libraryRepository;

        public AuthorContactController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult Index(Guid authorId)
        {
            if (!libraryRepository.AuthorExists(authorId))
                return NotFound();

            var contacs = libraryRepository.GetContactsForAuthor(authorId);

            var dtos = Mapper.Map<IEnumerable<AuthorContactDto>>(contacs);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetContactFromAuthor(Guid authorId, Guid id)
        {
            if (!libraryRepository.AuthorExists(authorId))
                return NotFound();

            var contact = libraryRepository.GetContactForAuthor(authorId, id);

            if (contact == null)
                return NotFound();

            var dto = Mapper.Map<AuthorContactDto>(contact);

            return Ok(dto);
        }
    }
}