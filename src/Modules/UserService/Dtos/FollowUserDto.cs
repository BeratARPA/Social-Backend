namespace UserService.Dtos
{
    public class FollowUserDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime FollowedAt { get; set; }
        public bool IsPrivate { get; set; }
    }
}
