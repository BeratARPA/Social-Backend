namespace AuthService.Dtos
{
    public class AuthResultDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Username { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
