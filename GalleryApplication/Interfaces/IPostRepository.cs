using System.Collections.Generic;
using System.Threading.Tasks;
using GalleryApplication.Models;

namespace GalleryApplication.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> GetPostByIdAsync(int id);
        void AddPost(Post post);
        void DeletePost(Post post);
        Task<List<Post>> GetPostsForUser(string username);
        Task<List<Post>> GetPostsWithComplaints();

    }
}