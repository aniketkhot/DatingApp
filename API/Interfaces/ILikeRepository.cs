using API.DTOs;
using API.Entities;
using API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ILikeRepository
    {
        Task<bool> AddLike(int targetUserId, int sourceUserId);
        Task<IEnumerable<LikeDto>> GetLikedUsers(LikeParams userParams);

        Task<IEnumerable<LikeDto>> GetLikedByUsers(LikeParams userParams);

        Task<bool> RemoveLike(int sourceUserId, int targetUserId);
    }
}
