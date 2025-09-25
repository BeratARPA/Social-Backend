namespace AuthService.Dtos
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
    }
}
