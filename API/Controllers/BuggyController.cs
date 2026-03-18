using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController: BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }
        
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }
        [Authorize]
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var x = _context.Users.Find(-1);
            if(x == null) return NotFound();
            return Ok(x); 
        }
        
        [Authorize]
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var x = _context.Users.Find(-1);
            var ret = x.ToString(); // NullReferenceException
            return ret;
        }
        [Authorize]
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("*sniff sniff* smells bad af");
        }
    }
}