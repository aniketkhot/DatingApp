using API.DTOs;
using API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface ILikeRepository
    {
        Task<bool> AddLike(int targetUserId, int sourceUserId);
        Task<IEnumerable<LikeDto>> GetLikedUsers(int sourceUserId);

        Task<IEnumerable<LikeDto>> GetLikedByUsers(int sourceUserId);

        Task<bool> RemoveLike(int sourceUserId, int targetUserId);
    }
}
