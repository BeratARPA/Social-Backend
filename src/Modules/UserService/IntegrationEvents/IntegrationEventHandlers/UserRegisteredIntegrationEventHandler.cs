using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents.Registered;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.IntegrationEvents.IntegrationEventHandlers
{
    public class UserRegisteredIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
    {
        private readonly IGenericRepository<UserProfile> _userProfileRepository;

        public UserRegisteredIntegrationEventHandler(IGenericRepository<UserProfile> userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public async Task Handle(UserRegisteredIntegrationEvent @event)
        {
            var profile = new UserProfile
            {
                UserId = @event.UserId,
                Username = @event.Username                
            };

            await _userProfileRepository.AddAsync(profile);
            await _userProfileRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
