using GalleryApplication.Interfaces;
using GalleryApplication.Models;

namespace GalleryApplication.Data
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _context;

        public LikeRepository(DataContext context)
        {
            _context = context;
        }

        public void AddLike(Like like)
        {
            _context.Likes.Add(like);
        }

        public void DeleteLike(Like like)
        {
            _context.Likes.Remove(like);
        }
    }
}