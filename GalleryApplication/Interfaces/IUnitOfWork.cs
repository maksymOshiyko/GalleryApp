using System.Threading.Tasks;

namespace GalleryApplication.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ICountryRepository CountryRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}