using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalleryApplication.Interfaces;
using GalleryApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace GalleryApplication.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext _context;

        public PostRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Post> GetPostByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Sender)
                .Include(p => p.Likes).ThenInclude(l => l.LikeSender)
                .Include(p => p.User)
                .SingleOrDefaultAsync(p => p.PostId == id);
        }

        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public void DeletePost(Post post)
        {
            _context.Posts.Remove(post);
        }

        public async Task<List<Post>> GetPostsForUser(string username)
        {
            var user = await _context.Users
                .Include(f => f.FollowedUsers).ThenInclude(f => f.SourceUser)
                .Include(f => f.FollowedUsers).ThenInclude(f => f.FollowedUser)
                .ThenInclude(u => u.Posts).ThenInclude(p => p.Likes)
                .Include(p => p.Posts).ThenInclude(p => p.Likes)
                .Include(p => p.Posts).ThenInclude(p => p.Comments).ThenInclude(c => c.Sender)
                .Include(p => p.Posts).ThenInclude(p => p.User)
                .SingleOrDefaultAsync(x => x.UserName == username);

            if (!user.FollowedUsers.Any()) return new List<Post>(); 
            
            var posts = user.FollowedUsers
                .Select(x => x.FollowedUser.Posts)
                .Aggregate((current, list) => current.Concat(list).ToList())
                .OrderByDescending(x => x.Created)
                .ToList();
            
            return posts;
        }
    }
}