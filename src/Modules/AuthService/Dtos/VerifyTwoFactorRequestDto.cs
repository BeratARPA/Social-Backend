namespace AuthService.Dtos
{
    public class VerifyTwoFactorRequestDto
    {
        public string TwoFactorToken { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
    }
}
