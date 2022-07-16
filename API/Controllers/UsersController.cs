using API.Data;
using API.DTOs;
using API.Entities;
using API.Extentions;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {

        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IPhotoService photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await userRepository.GetMemberByNameAsync(User.GetUsername());
            userParams.userId = user.Id;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }

            var pagedUsers = await this.userRepository.GetSortedFilteredMembersAsync(userParams);

            Response.AddPaginationHeader(pagedUsers.TotalCount, pagedUsers.PageSize, pagedUsers.CurrentPage, pagedUsers.TotalPages);

            return Ok(pagedUsers);         
        }


        [HttpGet("{username}", Name ="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await this.userRepository.GetMemberByNameAsync(username);           
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var user = await userRepository.GetUserByNameAsync(User.GetUsername());

            mapper.Map(memberUpdateDto, user);

            userRepository.Update(user);

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("cannot update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await userRepository.GetUserByNameAsync(User.GetUsername());
            var result = await photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

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

            if (await userRepository.SaveAllAsync())
            //return mapper.Map<PhotoDto>(photo);
            {
                return CreatedAtRoute("GetUser", new {username = user.UserName }, mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("cannot save photo to database");

        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await userRepository.GetUserByNameAsync(User.GetUsername());
            var currentMain =  user.Photos.FirstOrDefault(p => p.IsMain);
            var toBeMain = user.Photos.FirstOrDefault(p => p.Id == photoId);

            if (toBeMain == null) return BadRequest("Cannot set Photo as Main as it was not found");

            if (toBeMain.IsMain) return BadRequest("This Photo is already Main");

            if(currentMain != null)
            {
                currentMain.IsMain = false;
            }

            toBeMain.IsMain = true;

            if (await userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("cannot set Main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userRepository.GetUserByNameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("Cannot delete the Main photo");

            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) return BadRequest(result.Error.Message);

            user.Photos.Remove(photo);

            if (await userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Cannot delete photo");
        }
    }
}