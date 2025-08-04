namespace UserService.Data.Entities
{
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; } // AuthService tarafındaki referans

        public string Username { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }

        public bool IsPrivate { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
    }
}
