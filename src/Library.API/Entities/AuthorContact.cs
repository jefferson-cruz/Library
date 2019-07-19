using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Entities
{
    public class AuthorContact
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public ContactType Type { get; set; }

        [MaxLength(200)]
        public string Contact { get; set; }

        [ForeignKey("AuthorId")]
        public Author Author { get; set; }

        public Guid AuthorId { get; set; }
    }
}
