using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace GalleryApplication.Models
{
    public class AppUser : IdentityUser<int>
    {
        public AppUser()
        {
        }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int CountryId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Introduction { get; set; }
        public string MainPhotoUrl { get; set; }
        public string MainPhotoPublicId { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public ICollection<Follow> FollowedByUsers { get; set; }
        public ICollection<Follow> FollowedUsers { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}