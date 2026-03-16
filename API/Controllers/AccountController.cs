using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService; 
            _context = context;
            
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto regDto)
        {
            if(await exists(regDto.Username)) return Conflict(new {
                message = "Useranem already exists"
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
            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)  
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginAsync(LoginDto loginDto) 
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());
            if(user == null) return Unauthorized(new
            {
                message = "Invalid Username"
            });
            
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var givenHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            
            for(int i = 0; i < givenHash.Length; i++)
            {
                if(givenHash[i] != user.PasswordHash[i]) return Unauthorized(new
                {
                    message = "Invalid Password"
                });
            }

            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> exists(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

    }
}