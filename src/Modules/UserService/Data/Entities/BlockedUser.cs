namespace UserService.Data.Entities
{
    public class BlockedUser : BaseEntity
    {
        public Guid UserId { get; set; }
        public UserProfile User { get; set; } = null!;

        public Guid BlockedUserId { get; set; }
        public UserProfile Blocked { get; set; } = null!;
    }
}
