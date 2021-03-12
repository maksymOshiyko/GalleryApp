using System;

namespace GalleryApplication.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public DateTime? SentTime { get; set; } = DateTime.Now;
        public string Content { get; set; }
        public int? PostId { get; set; }
        public int? SenderId { get; set; }

        public virtual Post Post { get; set; }
        public virtual AppUser Sender { get; set; }
    }
}