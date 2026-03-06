using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
            
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> RegisterAsync(RegisterDto regDto)
        {
            if(await exists(regDto.Username)) return Conflict(new
            {
                message = "Username already exists"
            });
            
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = regDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(regDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user); // track that user with ef
            await _context.SaveChangesAsync(); // actually save it to db
            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> LoginAsync(LoginDto loginDto) 
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());
            if(user == null) return Unauthorized("Invalid Username");
            
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var givenHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            
            for(int i = 0; i < givenHash.Length; i++)
            {
                if(givenHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
            return user;
        }

        private async Task<bool> exists(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

    }
}