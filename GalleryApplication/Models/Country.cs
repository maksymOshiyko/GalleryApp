using System.Collections.Generic;

namespace GalleryApplication.Models
{
    public class Country
    {
        public Country()
        {
        }

        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public virtual ICollection<AppUser> Users { get; set; }
    }
}