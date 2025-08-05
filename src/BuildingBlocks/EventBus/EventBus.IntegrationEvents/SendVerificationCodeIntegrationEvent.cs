using EventBus.Base.Events;

namespace EventBus.IntegrationEvents
{
    public class SendVerificationCodeIntegrationEvent : IntegrationEvent
    {
        public VerificationChannel Channel { get; set; }
        public string Recipient { get; set; }
        public string Code { get; set; }

        public SendVerificationCodeIntegrationEvent(VerificationChannel channel, string recipient, string code)
        {
            Channel = channel;
            Recipient = recipient;
            Code = code;
        }
    }
}
