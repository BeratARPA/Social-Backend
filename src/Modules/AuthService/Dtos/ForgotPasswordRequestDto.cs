namespace AuthService.Dtos
{
    public class ForgotPasswordRequestDto
    {
        public string ActionToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
