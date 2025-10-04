using EventBus.Base.Events;

namespace EventBus.IntegrationEvents
{
    public class UserAccountRestoredIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
        public DateTime RestoredAt { get; set; }

        public UserAccountRestoredIntegrationEvent(Guid userId, DateTime restoredAt)
        {
            UserId = userId;
            RestoredAt = restoredAt;
        }
    }
}
