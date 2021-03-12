using GalleryApplication.Models;

namespace GalleryApplication.Interfaces
{
    public interface ICommentRepository
    {
        void AddComment(Comment comment);
        void DeleteComment(Comment comment);
    }
}