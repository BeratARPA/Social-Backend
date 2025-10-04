namespace AuthService.Dtos
{
    public class VerifyCodeResultDto
    {
        public bool IsSuccess { get; set; }
        public string ActionToken { get; set; } = string.Empty;
    }
}
