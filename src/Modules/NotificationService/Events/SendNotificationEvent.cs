using EventBus.Base.Events;

namespace NotificationService.Events
{
    public enum NotificationType
    {
        Email,
        Sms
    }

    public class SendNotificationEvent : IntegrationEvent
    {
        public NotificationType Type { get; set; }
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string TemplateName { get; set; }
        public object TemplateModel { get; set; }

        public SendNotificationEvent(NotificationType type, string recipient, string subject, string templateName, object model)
        {
            Type = type;
            Recipient = recipient;
            Subject = subject;
            TemplateName = templateName;
            TemplateModel = model;
        }
    }
}
