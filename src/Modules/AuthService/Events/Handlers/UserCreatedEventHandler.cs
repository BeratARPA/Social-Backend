using MediatR;
using Microsoft.Extensions.Logging;

namespace AuthService.Events.Handlers
{
    public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
    {
        private readonly ILogger<UserCreatedEventHandler> _logger;

        public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Yeni kullanıcı oluşturuldu: ID = {notification.UserId}, Email = {notification.Email}");
            return Task.CompletedTask;
        }
    }
}
