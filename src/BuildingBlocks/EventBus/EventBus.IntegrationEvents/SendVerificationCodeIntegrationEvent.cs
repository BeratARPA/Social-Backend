using EventBus.Base.Events;

namespace EventBus.IntegrationEvents
{
    public class SendVerificationCodeIntegrationEvent : IntegrationEvent
    {
        public VerificationChannel VerificationChannel { get; set; }
        public VerificationType VerificationType { get; set; }
        public string Recipient { get; set; }
        public string Code { get; set; }

        public SendVerificationCodeIntegrationEvent(VerificationChannel verificationChannel, VerificationType verificationType, string recipient, string code)
        {
            VerificationChannel = verificationChannel;
            VerificationType = verificationType;
            Recipient = recipient;
            Code = code;
        }
    }
}
