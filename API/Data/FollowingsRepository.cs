using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class FollowingsRepository : IFollowingsRepository
    {
        private readonly DataContext _context;
        public FollowingsRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserFollow> GetUserFollow(int sourceUserId, int followedUserId)
        {
            return await _context.Followings.FindAsync(sourceUserId, followedUserId);
        }

        // predicate => either user followers or user followed by 
        public async Task<PagedList<FollowDto>> GetUserFollowings(FollowingsParams followingsParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable(); 
            var followings = _context.Followings.AsQueryable();
            
            if(followingsParams.Predicate == "followed")
            {
                followings = followings.Where(follow => follow.SourceUserId == followingsParams.UserId);
                users = followings.Select(follow => follow.FollowedUesr);
            }
            if(followingsParams.Predicate == "followedBy")
            {
                followings = followings.Where(follow => follow.FollowedUserId == followingsParams.UserId);
                users = followings.Select(follow => follow.SourceUser);
            }
            var followedUsers = users.Select(user => new FollowDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<FollowDto>.CreateAsync(followedUsers, followingsParams.PageNumber, followingsParams.PageSize);
        }

        public async Task<AppUser> GetUserWithFollowings(int userId)
        {
            return await _context.Users
                .Include(u => u.FollowedUsers)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}