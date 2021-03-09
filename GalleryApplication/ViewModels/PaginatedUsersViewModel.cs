using System.Collections;
using System.Collections.Generic;
using GalleryApplication.Helpers;
using GalleryApplication.Models;

namespace GalleryApplication.ViewModels
{
    public class PaginatedUsersViewModel
    {
        public PageViewModel PageViewModel { get; set; }
        public IEnumerable<AppUser> Users { get; set; }
        public string Country { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string CurrentUsername { get; set; }
    }
}