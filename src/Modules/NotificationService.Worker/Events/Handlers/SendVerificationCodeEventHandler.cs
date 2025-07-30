using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents.Verification;
using NotificationService.Worker.Services;

namespace NotificationService.Worker.Events.Handlers
{
    public class SendVerificationCodeEventHandler : IIntegrationEventHandler<SendVerificationCodeIntegrationEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ITemplateRenderer _templateRenderer;

        public SendVerificationCodeEventHandler(
            IEmailSender emailSender,
            ISmsSender smsSender,
            ITemplateRenderer templateRenderer)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _templateRenderer = templateRenderer;
        }

        public async Task Handle(SendVerificationCodeIntegrationEvent @event)
        {
            var templateModel = new { Code = @event.Code };
            var body = await _templateRenderer.RenderAsync("VerificationCode", templateModel);

            switch (@event.Channel)
            {
                case VerificationChannel.Email:
                    await _emailSender.SendAsync(@event.Recipient, "Doğrulama Kodu", body);
                    break;

                case VerificationChannel.Sms:
                    await _smsSender.SendAsync(@event.Recipient, @event.Code); // SMS için sade içerik
                    break;

                default:
                    throw new InvalidOperationException("Desteklenmeyen kanal tipi");
            }
        }
    }
}