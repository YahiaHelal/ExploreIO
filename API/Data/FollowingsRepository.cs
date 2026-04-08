using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Data
{
    public class FollowingsRepository : IFollowingsRepository
    {
        private readonly DataContext _context;
        public FollowingsRepository(DataContext context)
        {
            _context = context;
        }
        public Task<UserFollow> GetUserFollow(int sourceUserId, int followedUserId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FollowDto>> GetUserFollowings(string predicate, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<AppUser> GetUserWithFollowings(int userId)
        {
            throw new NotImplementedException();
        }
    }
}