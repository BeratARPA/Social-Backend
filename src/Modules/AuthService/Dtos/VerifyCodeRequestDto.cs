using EventBus.IntegrationEvents;

namespace AuthService.Dtos
{
    public class VerifyCodeRequestDto
    {
        public VerificationChannel VerificationChannel { get; set; }
        public VerificationType VerificationType { get; set; }
        public string Target { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
