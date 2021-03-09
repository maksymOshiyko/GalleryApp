namespace GalleryApplication.Models
{
    public class Like
    {
        public int LikeId { get; set; }
        public int? PostId { get; set; }
        public int LikeSenderId { get; set; }

        public virtual AppUser LikeSender { get; set; }
        public virtual Post Post { get; set; }
    }
}