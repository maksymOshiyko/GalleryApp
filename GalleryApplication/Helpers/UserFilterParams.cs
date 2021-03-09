namespace GalleryApplication.Helpers
{
    public class UserFilterParams
    {
        public string Country { get; set; } = "All countries";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; } = "Male or female";
        public string CurrentUsername { get; set; }
    }
}