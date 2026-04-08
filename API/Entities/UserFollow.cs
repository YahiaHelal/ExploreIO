namespace API.Entities
{
    public class UserFollow
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser FollowedUesr { get; set; }
        public int FollowedUserId { get; set; }
    }
}