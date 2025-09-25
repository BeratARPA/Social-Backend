using EventBus.IntegrationEvents;

namespace AuthService.Dtos
{
    public class SendVerificationRequestDto
    {
        public VerificationChannel VerificationChannel { get; set; }
        public VerificationType VerificationType { get; set; }
        public string Target { get; set; } = string.Empty;
    }
}
