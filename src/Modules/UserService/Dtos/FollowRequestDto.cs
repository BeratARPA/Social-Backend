namespace UserService.Dtos
{
    public class FollowRequestDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
