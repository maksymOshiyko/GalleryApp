using System.Threading.Tasks;

namespace GalleryApplication.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}