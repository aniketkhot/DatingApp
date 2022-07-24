using API.Entities;
using API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Extentions;
using API.Helper;

namespace API.Data
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext dataContext;

        public LikeRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<bool> AddLike(int targetUserId, int sourceUserId)
        {
            AppUser sourceUser = await this.dataContext.Users
                                                    .Include(u => u.LikedUsers)
                                                    .FirstOrDefaultAsync(u => u.Id == sourceUserId);
            if (sourceUser == null) return false;
            AppUser targetUser = await this.dataContext.Users.FindAsync(targetUserId);
            if (targetUser == null) return false;
            var like = await this.dataContext.Likes.FindAsync(targetUserId, sourceUserId);
            if (like != null) return false;
            like = new Like { SourceUserId = sourceUserId, TargetUserId = targetUserId };

            sourceUser.LikedUsers.Add(like);


            return await this.dataContext.SaveChangesAsync() >= 1;
        }

        public async Task<IEnumerable<LikeDto>> GetLikedUsers(LikeParams likeParams)
        {
            var appUsers = this.dataContext.Users.AsQueryable()
                                            .Where(u => u.Id == likeParams.Id)
                                            .Include(u => u.LikedUsers)
                                            .ThenInclude(lu => lu.TargetUser)
                                            .SelectMany(u => u.LikedUsers.Select(l => l.TargetUser));

            var likedUsers = appUsers.Select(l => new LikeDto
            {
                Id = l.Id,
                Age = l.DateOfBirth.CalculateAge(),
                City = l.City,
                KnownAs = l.KnownAs,
                PhotoUrl = l.Photos.FirstOrDefault(p => p.IsMain).Url,
                Username = l.UserName
            });

            var uagedUsers = await PagedList<LikeDto>.CreateAsync(likedUsers, likeParams.PageSize, likeParams.PageNumber);
            return uagedUsers;
        }

        public async Task<IEnumerable<LikeDto>> GetLikedByUsers(LikeParams likeParams)
        {
            var query = this.dataContext.Users.AsQueryable()
                                        .Where(u => u.Id == likeParams.Id)
                                        .Include(u => u.LikedByUsers)
                                        .SelectMany(u => u.LikedByUsers.Select(l => l.SourceUser));


            var user = query.Select(u => new LikeDto
            {
                Id = u.Id,
                Age = u.DateOfBirth.CalculateAge(),
                City = u.City,
                KnownAs = u.KnownAs,
                PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain).Url,
                Username = u.UserName
            }
            );
            var users = await PagedList<LikeDto>.CreateAsync(user, likeParams.PageSize, likeParams.PageNumber);

            return users;
        }

        public async Task<bool> RemoveLike(int sourceUserId, int targetUserId)
        {
            var l = await this.dataContext.Likes.FindAsync(targetUserId, sourceUserId);
            var like = await this.dataContext.Users.AsQueryable()
                                        .Include(u => u.LikedUsers)
                                        .Where(u => u.Id == sourceUserId)
                                       .Select(u => u.LikedUsers)
                                       .SelectMany(u => u.Where(l => l.SourceUserId == sourceUserId && l.TargetUserId == targetUserId)).FirstOrDefaultAsync();

            var user = await this.dataContext.Users
                                        .Include(u => u.LikedUsers)
                                        .FirstOrDefaultAsync(u => u.Id == sourceUserId);
            if (l == null) return false;
            var removed = this.dataContext.Likes.Remove(l);
            return await this.dataContext.SaveChangesAsync() >= 1;
        }
    }
}
