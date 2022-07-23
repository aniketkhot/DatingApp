using API.DTOs;
using API.Entities;
using API.Helper;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }



        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await context.Users
                .Include(u => u.Photos)
                .ToListAsync();
        }

        public async Task<AppUser> GetUserByNameAsync(string name)
        {
            return await context.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.UserName == name);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() >= 1;
        }

        public void Update(AppUser user)
        {
            context.Entry(user).State = EntityState.Modified;
        }

        public async Task<PagedList<MemberDto>> GetAllMembersAsync(UserParams userParams)
        {
            var query = context.Users.AsQueryable()
                .Where(m => m.Gender == userParams.Gender)
                .Where(m => m.Id != userParams.userId)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .AsNoTracking();

            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageSize, userParams.PageNumber);

        }

        public async Task<PagedList<MemberDto>> GetSortedFilteredMembersAsync(UserParams userParams)
        {
            var flag = context.Users.First();
            var a = (flag.DateOfBirth - System.DateTime.Now).TotalDays > userParams.MinAge;                       

            var query = context.Users.AsQueryable();
            query = query.Where(m => m.Id != userParams.userId);
            query = query.Where(m => m.Gender == userParams.Gender);

            //code copied from course
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(m => m.DateOfBirth >= minDob && m.DateOfBirth <= maxDob);

            // TODO: for Neil - may be because of this line is is failing
            //query = query.Where(m => (m.DateOfBirth - System.DateTime.Now).TotalDays / 365 > userParams.MinAge);

            query = userParams.OrderBy switch
            {
                "Created" => query.OrderByDescending(m => m.Created),
                "LastActive" => query.OrderByDescending(m => m.LastActive),
                _ => query.OrderByDescending(m => m.LastActive)
            };

            IQueryable<MemberDto> membersQuery = query.ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .AsNoTracking();



            return await PagedList<MemberDto>.CreateAsync(membersQuery, userParams.PageSize, userParams.PageNumber);

        }

        public async Task<MemberDto> GetMemberByNameAsync(string name)
        {
            return await context.Users
                .Where(u => u.UserName == name)
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

        }
    }
}
