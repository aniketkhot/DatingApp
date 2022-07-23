using API.DTOs;
using API.Extentions;
using API.Helper;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly ILikeRepository likeRepository;

        public LikesController(IUserRepository userRepository, ILikeRepository likeRepository)
        {
            this.userRepository = userRepository;
            this.likeRepository = likeRepository;
        }

        [HttpGet("liked")]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(LikeParams likeParams)
        {
            int userId = User.GetUserId();
            IEnumerable<LikeDto> likes = await this.likeRepository.GetLikedUsers(userId);
            return Ok(likes);
        }

        [HttpGet("likedby")]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetLikedByUsers(LikeParams likeParams)
        {
            var member = await this.userRepository.GetMemberByNameAsync(likeParams.username);
            int userId = member.Id;
            IEnumerable<LikeDto> likes = await this.likeRepository.GetLikedByUsers(userId);
            return Ok(likes);
        }

        [HttpPost("{targetUsername}")]
        public async void AddLike(string targetUsername)
        {
            int sourceId = User.GetUserId();

            MemberDto member = await this.userRepository.GetMemberByNameAsync(targetUsername);
            int targetId = member.Id;
            await this.likeRepository.AddLike(targetId, sourceId);
        }

        [HttpPost("removeLike/{targetUsername}")]
        public async void Remove(string targetUsername)
        {
            int sourceId = User.GetUserId();
            MemberDto member = await this.userRepository.GetMemberByNameAsync(targetUsername);
            int targetId = member.Id;
            await this.likeRepository.RemoveLike(sourceId, targetId);
        }
    }
}
