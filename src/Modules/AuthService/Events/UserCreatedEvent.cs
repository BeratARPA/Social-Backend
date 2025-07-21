using MediatR;

namespace AuthService.Events
{
    public class UserCreatedEvent : INotification
    {
        public Guid UserId { get; }
        public string Email { get; }

        public UserCreatedEvent(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
