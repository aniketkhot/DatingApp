using API.DTOs;
using API.Entities;
using API.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByNameAsync(string name);

        Task<MemberDto> GetMemberByNameAsync(string name);

        Task<PagedList<MemberDto>> GetAllMembersAsync( UserParams userParams);

        Task<PagedList<MemberDto>> GetSortedFilteredMembersAsync(UserParams userParams);

    }
}
