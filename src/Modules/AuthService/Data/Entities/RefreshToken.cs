namespace AuthService.Data.Entities
{
    public class RefreshToken : BaseEntity
    {
        public Guid UserCredentialId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string? CreatedByIp { get; set; }
        public bool IsRevoked { get; set; }
        public string? RevokedByIp { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? UserAgent { get; set; }

        public UserCredential UserCredential { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    }
}
