using System.Threading.Tasks;

namespace GalleryApplication.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ICountryRepository CountryRepository { get; }
        IPostRepository PostRepository { get; }
        ILikeRepository LikeRepository { get; }
        ICommentRepository CommentRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}