using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs // DTOs are the objects sent, forms a subset from the entity it represents
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }


        [Required]
        [StringLength(15, MinimumLength = 4)]
        public string Password { get; set; }
    }
}