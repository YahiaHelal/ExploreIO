using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsersAsync([FromQuery] UserParams userParams)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            userParams.CurrentUsername = user.UserName;
            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUserAsync(string username)
        {
            return await _unitOfWork.UserRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdate)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            _mapper.Map(memberUpdate, user);
            _unitOfWork.UserRepository.Update(user);

            if(await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error != null) return BadRequest(result.Error.Message);
            
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if(await _unitOfWork.Complete())
            {
                return CreatedAtRoute("GetUser", new {username = user.UserName}, _mapper.Map<PhotoDto>(photo));
            }
            return BadRequest("A problem occured while adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            
            if(photo.IsMain) return BadRequest("This is already your main photo");
            
            var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);
            if(currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if(await _unitOfWork.Complete()) return NoContent();
            return BadRequest("Failed to set main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if(photo == null) return NotFound();
            
            if(photo.IsMain) return BadRequest("You cannot delete your main photo");
            
            if(photo.PublicId != null)
            {
                var delResult = await _photoService.DeletePhotoAsync(photo.PublicId);
                
                if(delResult.Error != null) // failed to delete from cloudinary, don't delete from db
                {
                    return BadRequest(delResult.Error.Message);
                }
            }
            user.Photos.Remove(photo);
            if(await _unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to delete photo");
        }


        // TODO: delete user account
    }
}