using EventBus.Base.Events;

namespace EventBus.IntegrationEvents
{
    public class UserAccountSoftDeletedIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
        public DateTime DeletedAt { get; set; }
        public DateTime ScheduledForHardDeleteAt { get; set; }

        public UserAccountSoftDeletedIntegrationEvent(Guid userId, DateTime deletedAt, DateTime scheduledForHardDeleteAt)
        {
            UserId = userId;
            DeletedAt = deletedAt;
            ScheduledForHardDeleteAt = scheduledForHardDeleteAt;
        }
    }
}
