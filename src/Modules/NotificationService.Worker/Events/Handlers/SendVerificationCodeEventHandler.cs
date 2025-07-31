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
            _logger.LogInformation("=== [Handler] Event alındı! ===");
            _logger.LogInformation("[Handler] Recipient: {@Recipient}", @event.Recipient);
            _logger.LogInformation("[Handler] Channel: {@Channel}", @event.Channel);
            _logger.LogInformation("[Handler] Code: {@Code}", @event.Code);

            try
            {
                Console.WriteLine($"[Handler] Event alındı: {@event.Recipient}, Kanal: {@event.Channel}");

                var templateModel = new { Code = @event.Code };
                var body = await _templateRenderer.RenderAsync("VerificationCode", templateModel);

                switch (@event.Channel)
                {
                    case VerificationChannel.Email:
                        _logger.LogInformation("[Handler] Email gönderiliyor: {Recipient}", @event.Recipient);
                        Console.WriteLine($"[Handler] Email gönderiliyor: {@event.Recipient}");
                        await _emailSender.SendAsync(@event.Recipient, "Doğrulama Kodu", body);
                        _logger.LogInformation("[Handler] Email başarıyla gönderildi!");
                        Console.WriteLine($"[Handler] Email başarıyla gönderildi");
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
                _logger.LogError(ex, "[Handler] HATA oluştu");
                Console.WriteLine($"[Handler] HATA: {ex.Message}");
                Console.WriteLine($"[Handler] Stack: {ex.StackTrace}");
                throw; // Re-throw to let EventBus handle
            }
        }
    }
}