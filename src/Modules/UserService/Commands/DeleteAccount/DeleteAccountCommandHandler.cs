using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.DeleteAccount
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
    {
        private readonly IGenericRepository<UserProfile> _userProfileRepository;
        private readonly IGenericRepository<Follow> _followRepository;
        private readonly IGenericRepository<BlockedUser> _blockedUserRepository;
        private readonly IEventBus _eventBus;

        public DeleteAccountCommandHandler(
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

        public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcı profilini bul
            var user = await _userProfileRepository.GetByIdAsync(request.UserId);
            if (user is null)
                throw new NotFoundException("UserNotFound");

            // Zaten silinmiş mi kontrol et
            if (user.IsDeleted)
                throw new ValidationException("UserAccountHasAlreadyBeenDeleted");

            // 1. UserProfile'i soft delete yap          
            await _userProfileRepository.SoftDeleteAsync(user.Id);

            // 2. Follow ilişkilerini soft delete yap
            var follows = await _followRepository.GetAsync(
                filter: f => f.FollowerId == request.UserId || f.FolloweeId == request.UserId
            );

            foreach (var follow in follows)
            {              
                await _followRepository.SoftDeleteAsync(follow.Id);
            }

            // 3. BlockedUser kayıtlarını soft delete yap
            var blockedUsers = await _blockedUserRepository.GetAsync(
                filter: b => b.UserId == request.UserId || b.BlockedUserId == request.UserId
            );

            foreach (var blockedUser in blockedUsers)
            {             
                await _blockedUserRepository.SoftDeleteAsync(blockedUser.Id);
            }

            // 4. Değişiklikleri kaydet
            await _userProfileRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await _followRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await _blockedUserRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Diğer servislere haber ver (PostService, CommentService vs.)
            _eventBus.Publish(new UserAccountSoftDeletedIntegrationEvent(
                request.UserId,
                user.DeletedAt!.Value,
                user.ScheduledForHardDeleteAt!.Value
            ));

            return true;
        }
    }
}