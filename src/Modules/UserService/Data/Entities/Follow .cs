namespace UserService.Data.Entities
{
    public class Follow : BaseEntity
    {
        public Guid FollowerId { get; set; }
        public UserProfile Follower { get; set; } = null!;

        public Guid FolloweeId { get; set; }
        public UserProfile Followee { get; set; } = null!;

        public bool IsAccepted { get; set; } = false; // onaylı mı
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
    }
}
