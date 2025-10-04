using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using NotificationService.Services;

namespace NotificationService.Events.Handlers
{
    public class SendVerificationCodeEventHandler : IIntegrationEventHandler<SendVerificationCodeIntegrationEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IWhatsAppSender _whatsAppSender;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly ILogger<SendVerificationCodeEventHandler> _logger;

        public SendVerificationCodeEventHandler(
            IEmailSender emailSender,
            ISmsSender smsSender,
            IWhatsAppSender whatsAppSender,
            ITemplateRenderer templateRenderer,
            ILogger<SendVerificationCodeEventHandler> logger)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _whatsAppSender = whatsAppSender;
            _templateRenderer = templateRenderer;
            _logger = logger;
        }

        public async Task Handle(SendVerificationCodeIntegrationEvent @event)
        {
            try
            {
                var templateModel = new { Code = @event.Code };
                var (templateName, subject) = GetTemplateDetail(@event.VerificationType);
                var body = await _templateRenderer.RenderAsync(templateName, templateModel);

                switch (@event.VerificationChannel)
                {
                    case VerificationChannel.Email:
                        await _emailSender.SendAsync(@event.Recipient, subject, body);
                        break;

                    case VerificationChannel.Sms:
                        await _smsSender.SendAsync(@event.Recipient, body);
                        break;

                    case VerificationChannel.WhatsApp:
                        await _whatsAppSender.SendAsync(@event.Recipient, body);
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

        private (string, string) GetTemplateDetail(VerificationType verificationType)
        {
            return verificationType switch
            {
                VerificationType.VerifyEmail => ("VerificationCodeEmail", "Email Doğrulama"),
                VerificationType.VerifyPhone => ("VerificationCodePhone", "Telefon Doğrulama"),
                VerificationType.ResetPassword => ("VerificationCodeResetPassword", "Şifre Sıfırlama"),
                VerificationType.TwoFactor => ("VerificationCodeTwoFactor", "İki Faktörlü Doğrulama"),
                _ => throw new InvalidOperationException("Desteklenmeyen doğrulama türü"),
            };
        }
    }
}