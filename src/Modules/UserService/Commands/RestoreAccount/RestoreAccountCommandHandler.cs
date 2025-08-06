using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.RestoreAccount
{
    public class RestoreAccountCommandHandler : IRequestHandler<RestoreAccountCommand, bool>
    {
        private readonly IGenericRepository<UserProfile> _userProfileRepository;
        private readonly IGenericRepository<Follow> _followRepository;
        private readonly IGenericRepository<BlockedUser> _blockedUserRepository;
        private readonly IEventBus _eventBus;

        public RestoreAccountCommandHandler(
            IGenericRepository<UserProfile> userProfileRepository,
            IGenericRepository<Follow> followRepository,
            IGenericRepository<BlockedUser> blockedUserRepository,
            IEventBus eventBus)
        {
            _userProfileRepository = userProfileRepository;
            _followRepository = followRepository;
            _blockedUserRepository = blockedUserRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(RestoreAccountCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcı profilini bul (silinmiş olanları da dahil et)
            var user = await _userProfileRepository.GetByIdIncludeDeletedAsync(request.UserId);

            if (user is null)
                throw new NotFoundException("UserNotFound");

            if (!user.IsDeleted)
                throw new ValidationException("UserAccountIsAlreadyActive");

            // Hard delete zamanı geçmiş mi kontrol et
            if (user.IsReadyForHardDelete())
                throw new ValidationException("AccountHasBeenPermanentlyDeleted");

            // 1. UserProfile'i restore et      
            await _userProfileRepository.RestoreAsync(user.Id);

            // 2. Follow ilişkilerini restore et
            var follows = await _followRepository.GetAsync(
                filter: f => (f.FollowerId == request.UserId || f.FolloweeId == request.UserId) && f.IsDeleted
            );

            foreach (var follow in follows)
            {               
                await _followRepository.RestoreAsync(follow.Id);
            }

            // 3. BlockedUser kayıtlarını restore et
            var blockedUsers = await _blockedUserRepository.GetAsync(
                filter: b => (b.UserId == request.UserId || b.BlockedUserId == request.UserId) && b.IsDeleted
            );

            foreach (var blockedUser in blockedUsers)
            {             
                await _blockedUserRepository.RestoreAsync(blockedUser.Id);
            }

            // 4. Değişiklikleri kaydet
            await _userProfileRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Diğer servislere haber ver
            _eventBus.Publish(new UserAccountRestoredIntegrationEvent(
                request.UserId,
                DateTime.UtcNow
            ));

            return true;
        }
    }
}
