using EventBus.Base.Events;

namespace EventBus.IntegrationEvents
{
    public class UserDeletedIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }

        public UserDeletedIntegrationEvent(Guid userId)
        {
            UserId = userId;
        }
    }
}
