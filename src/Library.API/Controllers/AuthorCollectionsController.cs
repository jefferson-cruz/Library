using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : Controller
    {
        private readonly ILibraryRepository libraryRepository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            this.libraryRepository = libraryRepository;
        }

        [HttpPost]
        public IActionResult CreateAuthorCollections([FromBody] IEnumerable<AuthorForCreationDto> dto)
        {
            if (dto == null)
                return BadRequest();

            var entities = Mapper.Map<IEnumerable<Author>>(dto);

            foreach (var author in entities)
                libraryRepository.AddAuthor(author);

            if (!libraryRepository.Save())
                throw new Exception("Creating author collection failed on save");

            var authorsToReturn = Mapper.Map<IEnumerable<AuthorDto>>(entities);

            return CreatedAtAction(
                nameof(GetAuthorCollection),
                new { ids = string.Join(",", authorsToReturn.Select(x => x.Id)) },
                authorsToReturn);
        }

        [HttpGet("({ids})")]
        public IActionResult GetAuthorCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
                return BadRequest();

            var authorEntities = libraryRepository.GetAuthors(ids);

            if (ids.Count() != authorEntities.Count())
                return NotFound();

            var authorsToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);

            return Ok(authorsToReturn);
        }
    }
}