namespace UserService.Data.Entities
{
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; } // AuthService tarafındaki referans

        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public string? Bio { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = string.Empty;
        public bool IsPrivate { get; set; } = true;
    }
}
