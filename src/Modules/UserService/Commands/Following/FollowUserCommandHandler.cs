using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.Following
{
    public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, bool>
    {
        private readonly IGenericRepository<UserProfile> _userRepository;
        private readonly IGenericRepository<Follow> _followRepository;

        public FollowUserCommandHandler(
            IGenericRepository<UserProfile> userRepository,
            IGenericRepository<Follow> followRepository)
        {
            _userRepository = userRepository;
            _followRepository = followRepository;
        }

        public async Task<bool> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            // Kendini takip etmeye çalışıyor mu?
            if (request.CurrentUserId == request.TargetUserId)
                throw new ValidationException("Kendinizi takip edemezsiniz.");

            // Hedef kullanıcı var mı?
            var targetUser = await _userRepository.GetByIdAsync(request.TargetUserId);
            if (targetUser == null)
                throw new NotFoundException("Takip edilmek istenen kullanıcı bulunamadı.");

            // Mevcut kullanıcı var mı?
            var currentUser = await _userRepository.GetByIdAsync(request.CurrentUserId);
            if (currentUser == null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            // Zaten takip ediyor mu? (kabul edilmiş veya bekleyen istek)
            var existingFollow = await _followRepository.FirstOrDefaultAsync(
                f => f.FollowerId == request.CurrentUserId && f.FolloweeId == request.TargetUserId
            );

            if (existingFollow != null)
            {
                if (existingFollow.IsAccepted)
                    throw new ValidationException("Bu kullanıcıyı zaten takip ediyorsunuz.");
                else
                    throw new ValidationException("Bu kullanıcıya zaten takip isteği gönderilmiş.");
            }

            // Yeni follow kaydı oluştur
            var follow = new Follow
            {
                FollowerId = request.CurrentUserId,
                FolloweeId = request.TargetUserId,
                RequestedAt = DateTime.UtcNow,
                // Eğer hedef kullanıcı private değilse otomatik kabul et
                IsAccepted = !targetUser.IsPrivate,
                AcceptedAt = !targetUser.IsPrivate ? DateTime.UtcNow : null
            };

            await _followRepository.AddAsync(follow);
            await _followRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
