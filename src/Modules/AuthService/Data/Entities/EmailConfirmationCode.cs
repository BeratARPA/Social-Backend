namespace AuthService.Data.Entities
{
    public class EmailConfirmationCode : BaseEntity
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public string? UsedByIp { get; set; }
        public DateTime? UsedAt { get; set; }
        public string CreatedByIp { get; set; } = null!;
        public string? UserAgent { get; set; }

        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
        public bool IsValid => !IsUsed && !IsExpired;
    }
}
