using System;
using System.ComponentModel.DataAnnotations;

namespace GalleryApplication.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(4), MaxLength(25)]
        public string UserName { get; set; }
        [Required]
        [MinLength(6), MaxLength(20)]
        public string Password { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
    }
}