using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController: BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService; 
            
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto regDto)
        {
            if(await exists(regDto.Username)) return Conflict("Useranem already exists");
            
            var user = _mapper.Map<AppUser>(regDto);

            user.UserName = regDto.Username.ToLower();
            
            var result = await _userManager.CreateAsync(user, regDto.Password);

            if(!result.Succeeded) return BadRequest(result.Errors);

            var roleRes = await _userManager.AddToRoleAsync(user, "Member");
            
            if(!roleRes.Succeeded) return BadRequest(result.Errors);

            return new UserDto()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user),
                KnownAs = user.KnownAs
            };
        }

        //BUG: invalid user creds throws 500
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginAsync(LoginDto loginDto) 
        {
            var user = await _userManager.Users
            .Include(u => u.Photos)
            .SingleOrDefaultAsync(u => !string.IsNullOrEmpty(loginDto.Username) && u.UserName == loginDto.Username.ToLower());
            if(user == null) return Unauthorized("Invalid Credintials");
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!result.Succeeded) return Unauthorized("Invalid Credintials");

            return new UserDto()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> exists(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }

    }
}