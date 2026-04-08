using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IFollowingsRepository
    {
        Task<UserFollow> GetUserFollow(int sourceUserId, int followedUserId);
        Task<AppUser> GetUserWithFollowings(int userId);
        Task<IEnumerable<FollowDto>> GetUserFollowings(string predicate, int userId); // followed / followed by user
    }
}