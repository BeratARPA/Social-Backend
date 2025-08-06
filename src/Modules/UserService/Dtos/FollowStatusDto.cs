namespace UserService.Dtos
{
    public class FollowStatusDto
    {
        public bool IsFollowing { get; set; }
        public bool IsFollowedBy { get; set; }
        public bool HasPendingRequest { get; set; }
        public bool HasIncomingRequest { get; set; }
        public DateTime? FollowedAt { get; set; }
        public DateTime? FollowRequestedAt { get; set; }
    }
}
