namespace GalleryApplication.Models
{
    public class Follower
    {
        public int SourceUserId { get; set; }
        public int FollowedUserId { get; set; }

        public AppUser FollowedUser { get; set; }
        public AppUser SourceUser { get; set; }
    }
}