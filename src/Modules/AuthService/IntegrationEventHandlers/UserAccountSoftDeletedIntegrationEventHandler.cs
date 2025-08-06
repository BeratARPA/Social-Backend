using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;

namespace AuthService.IntegrationEventHandlers
{
    public class UserAccountSoftDeletedIntegrationEventHandler : IIntegrationEventHandler<UserAccountSoftDeletedIntegrationEvent>
    {
        private readonly IGenericRepository<UserCredential> _userCredentialRepository;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly IEventBus _eventBus;

        public UserAccountSoftDeletedIntegrationEventHandler(
            IEventBus eventBus,
            IGenericRepository<UserCredential> userCredentialRepository,
            IGenericRepository<RefreshToken> refreshTokenRepository)
        {
            _eventBus = eventBus;
            _userCredentialRepository = userCredentialRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task Handle(UserAccountSoftDeletedIntegrationEvent @event)
        {
            // Kullanıcı profilini bul
            var user = await _userCredentialRepository.GetByIdAsync(@event.UserId);
            if (user is null)
                throw new NotFoundException("UserNotFound");

            // Zaten silinmiş mi kontrol et
            if (user.IsDeleted)
                throw new ValidationException("UserAccountHasAlreadyBeenDeleted");

            // 1. UserProfile'i soft delete yap          
            await _userCredentialRepository.SoftDeleteAsync(user.Id);

            // 2. Follow ilişkilerini soft delete yap
            var refreshTokens = await _refreshTokenRepository.GetAsync();
            foreach (var refreshToken in refreshTokens)
            {
                await _refreshTokenRepository.SoftDeleteAsync(refreshToken.Id);
            }

            // 3. Değişiklikleri kaydet
            await _userCredentialRepository.UnitOfWork.SaveChangesAsync();
            await _refreshTokenRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
