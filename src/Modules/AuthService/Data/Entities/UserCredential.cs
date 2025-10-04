namespace AuthService.Data.Entities
{
    public class UserCredential : BaseEntity
    {
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsEmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsPhoneConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutEnd { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
