using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.Following
{
    public class DeclineFollowRequestCommandHandler : IRequestHandler<DeclineFollowRequestCommand, bool>
    {
        private readonly IGenericRepository<Follow> _followRepository;
        private readonly IGenericRepository<UserProfile> _userRepository;

        public DeclineFollowRequestCommandHandler(
            IGenericRepository<Follow> followRepository,
            IGenericRepository<UserProfile> userRepository)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(DeclineFollowRequestCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcının private hesabı var mı kontrol et
            var currentUser = await _userRepository.GetByIdAsync(request.CurrentUserId);
            if (currentUser == null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            if (!currentUser.IsPrivate)
                throw new ValidationException("Sadece private hesaplar takip isteklerini reddedebilir.");

            // Bekleyen takip isteğini bul
            var followRequest = await _followRepository.FirstOrDefaultAsync(
                f => f.FollowerId == request.RequestUserId &&
                     f.FolloweeId == request.CurrentUserId &&
                     !f.IsAccepted
            );

            if (followRequest == null)
                throw new NotFoundException("Bekleyen takip isteği bulunamadı.");

            // İsteği reddet (soft delete)
            await _followRepository.SoftDeleteAsync(request.CurrentUserId);
            await _followRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
