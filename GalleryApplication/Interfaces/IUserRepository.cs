using System.Collections.Generic;
using System.Threading.Tasks;
using GalleryApplication.Helpers;
using GalleryApplication.Models;

namespace GalleryApplication.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetUsersAsync(UserFilterParams userFilterParams);
        Task<AppUser> GetUserByUsernameAsync(string username);
        Task FollowUser(AppUser sourceUser, AppUser followedUser);
        Task UnfollowUser(AppUser sourceUser, AppUser followedUser);
    }
}