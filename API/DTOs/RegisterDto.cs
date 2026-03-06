using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs // DTOs are the objects sent, forms a subset from the entity it represents
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}