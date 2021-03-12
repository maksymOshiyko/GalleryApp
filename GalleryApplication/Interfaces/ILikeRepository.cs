using GalleryApplication.Models;

namespace GalleryApplication.Interfaces
{
    public interface ILikeRepository
    {
        void AddLike(Like like);
        void DeleteLike(Like like);
    }
}