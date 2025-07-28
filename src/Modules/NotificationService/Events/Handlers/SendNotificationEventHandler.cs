using EventBus.Base.Abstraction;
using NotificationService.Services;

namespace NotificationService.Events.Handlers
{
    public class SendNotificationEventHandler : IIntegrationEventHandler<SendNotificationEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ITemplateRenderer _templateRenderer;

        public SendNotificationEventHandler(IEmailSender emailSender, ISmsSender smsSender, ITemplateRenderer templateRenderer)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _templateRenderer = templateRenderer;
        }

        public async Task Handle(SendNotificationEvent @event)
        {
            var body = await _templateRenderer.RenderAsync(@event.TemplateName, @event.TemplateModel);

            switch (@event.Type)
            {
                case NotificationType.Email:
                    await _emailSender.SendAsync(@event.Recipient, @event.Subject, body);
                    break;
                case NotificationType.Sms:
                    await _smsSender.SendAsync(@event.Recipient, body);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported notification type");
            }
        }
    }
}
