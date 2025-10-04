namespace AuthService.Dtos
{
    public class AuthResultDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public DateTime? ExpiresAt { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public string? TwoFactorToken { get; set; } = string.Empty;
    }
}
