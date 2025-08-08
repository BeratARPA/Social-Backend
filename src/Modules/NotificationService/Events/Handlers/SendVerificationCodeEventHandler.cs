using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using NotificationService.Services;

namespace NotificationService.Events.Handlers
{
    public class SendVerificationCodeEventHandler : IIntegrationEventHandler<SendVerificationCodeIntegrationEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly ILogger<SendVerificationCodeEventHandler> _logger;

        public SendVerificationCodeEventHandler(
            IEmailSender emailSender,
            ISmsSender smsSender,
            ITemplateRenderer templateRenderer,
            ILogger<SendVerificationCodeEventHandler> logger)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _templateRenderer = templateRenderer;
            _logger = logger;
        }

        public async Task Handle(SendVerificationCodeIntegrationEvent @event)
        {
            try
            {
                var templateModel = new { Code = @event.Code };
                var body = await _templateRenderer.RenderAsync("VerificationCode", templateModel);

                switch (@event.Channel)
                {
                    case VerificationChannel.Email:
                        await _emailSender.SendAsync(@event.Recipient, "Doğrulama Kodu", body);
                        break;

                    case VerificationChannel.Sms:
                        await _smsSender.SendAsync(@event.Recipient, @event.Code);
                        break;

                    default:
                        throw new InvalidOperationException("Desteklenmeyen kanal tipi");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                throw;
            }
        }
    }
}