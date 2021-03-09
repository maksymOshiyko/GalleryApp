using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GalleryApplication.Helpers;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GalleryApplication.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync(UserFilterParams userFilterParams)
        {
            var query = _context.Users
                .Include(c => c.Country)
                .AsQueryable();

            query = query.Where(u => u.UserName != userFilterParams.CurrentUsername);
            
            if (userFilterParams.Gender != "Male or female")
            {
                query = query.Where(u => u.Gender == userFilterParams.Gender); 
            }

            if (!string.IsNullOrEmpty(userFilterParams.FirstName))
            {
                query = query.Where(u => u.FirstName.Contains(userFilterParams.FirstName));
            }
            
            if (!string.IsNullOrEmpty(userFilterParams.LastName))
            {
                query = query.Where(u => u.LastName.Contains(userFilterParams.LastName));
            }
            
            if (userFilterParams.Country != "All countries")
            {
                query = query.Where(u => u.Country.CountryName == userFilterParams.Country);
            }

            return await query.ToListAsync();
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(f => f.FollowedUsers)
                .Include(f => f.FollowedByUsers)
                .Include(c => c.Country)
                .Include(c => c.Comments)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }
    }
}