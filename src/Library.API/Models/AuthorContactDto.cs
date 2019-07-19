using Library.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Models
{
    public class AuthorContactDto
    {
        public Guid Id { get; set; }

        public string Type { get; set; }

        public string Contact { get; set; }

        public Guid AuthorId { get; set; }
    }
}
