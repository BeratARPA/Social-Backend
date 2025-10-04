using EventBus.Base.Events;

namespace EventBus.IntegrationEvents
{
    public class UserRegisteredIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }

        public UserRegisteredIntegrationEvent(Guid userId, string username)
        {
            UserId = userId;
            Username = username;
        }
    }
}
