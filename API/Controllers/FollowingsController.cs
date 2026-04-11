using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class FollowingsController: BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowingsRepository _followingsRepository;
        public FollowingsController(IUserRepository userRepository, IFollowingsRepository followingsRepository)
        {
            _followingsRepository = followingsRepository;
            _userRepository = userRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddFollow(string username)
        {
            var sourceUserId = User.GetUserId();
            var sourceUser = await _followingsRepository.GetUserWithFollowings(sourceUserId);
            var followedUser = await _userRepository.GetUserByUsernameAsync(username);
            
            if(followedUser == null) return NotFound();
            if(sourceUser.UserName == username) return BadRequest("You cannot follow yourself");

            var userFollow = await _followingsRepository.GetUserFollow(sourceUserId, followedUser.Id);

            if(userFollow != null) return BadRequest("You're already following this user");

            userFollow = new UserFollow
            {
                SourceUserId = sourceUserId,
                FollowedUserId = followedUser.Id
            };

            sourceUser.FollowedUsers.Add(userFollow);
            if(await _userRepository.SaveAllAsync()) return Ok(); // should save from followings repo ?
            
            return BadRequest("Falied to follow user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FollowDto>>> GetUserFollowings([FromQuery]FollowingsParams followingsParams)
        {
            followingsParams.UserId = User.GetUserId();
            var users = await _followingsRepository.GetUserFollowings(followingsParams);
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }

        [HttpDelete("{username}")]
        public async Task<ActionResult> RemoveFollow(string username)
        {
            var sourceUserId = User.GetUserId();
            var sourceUser = await _followingsRepository.GetUserWithFollowings(sourceUserId);
            var followedUser = await _userRepository.GetUserByUsernameAsync(username);

            if(followedUser == null) return NotFound();
            if(sourceUser.UserName == username) return BadRequest("You cannot unfollow yourself");
            
            var userFollow = await _followingsRepository.GetUserFollow(sourceUserId, followedUser.Id);
            if(userFollow == null) return BadRequest("You're already not following this user");

            sourceUser.FollowedUsers.Remove(userFollow);
            if(await _userRepository.SaveAllAsync()) return Ok(); // also should save from followings repo
            return BadRequest("Failed to unfollow user");
        }
        

        //TODO: implement unfollow option
    }
}