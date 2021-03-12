using Microsoft.AspNetCore.Http;

namespace GalleryApplication.ViewModels
{
    public class AddPostViewModel
    {
        public string Description { get; set; }
        public IFormFile File { get; set; }
        public string Error { get; set; }
    }
}