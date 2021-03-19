using System;
using System.Collections.Generic;

namespace GalleryApplication.Models
{
    public class Post
    {
        public Post()
        {
        }

        public int PostId { get; set; }
        public int? UserId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoPublicId { get; set; }
        public bool HasComplaint { get; set; }
        public virtual AppUser User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}