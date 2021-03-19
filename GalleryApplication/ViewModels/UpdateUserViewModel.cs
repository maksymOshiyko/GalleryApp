using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GalleryApplication.ViewModels
{
    public class UpdateUserViewModel
    {
        [MinLength(2), MaxLength(20)]
        public string FirstName { get; set; }
        [MinLength(2), MaxLength(20)]
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string DateOfBirth { get; set; }
        public IFormFile Image { get; set; }
    }
}