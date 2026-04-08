using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
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
        public async Task<IEnumerable<FollowDto>> GetUserFollowings(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable(); 
            var followings = _context.Followings.AsQueryable();
            
            if(predicate == "followed")
            {
                followings = followings.Where(follow => follow.SourceUserId == userId);
                users = followings.Select(follow => follow.FollowedUesr);
            }
            if(predicate == "followedBy")
            {
                followings = followings.Where(follow => follow.FollowedUserId == userId);
                users = followings.Select(follow => follow.SourceUser);
            }
            return await users.Select(user => new FollowDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithFollowings(int userId)
        {
            return await _context.Users
                .Include(u => u.FollowedUsers)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}