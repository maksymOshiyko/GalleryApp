using System.ComponentModel.DataAnnotations;

namespace GalleryApplication.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmNewPassword { get; set; }
    }
}