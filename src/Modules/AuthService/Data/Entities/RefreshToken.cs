namespace AuthService.Data.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public Guid UserCredentialId { get; set; }

        public UserCredential UserCredential { get; set; } = null!;
    }
}
