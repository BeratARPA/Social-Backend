using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.Following
{
    public class AcceptFollowRequestCommandHandler : IRequestHandler<AcceptFollowRequestCommand, bool>
    {
        private readonly IGenericRepository<Follow> _followRepository;
        private readonly IGenericRepository<UserProfile> _userRepository;

        public AcceptFollowRequestCommandHandler(
            IGenericRepository<Follow> followRepository,
            IGenericRepository<UserProfile> userRepository)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(AcceptFollowRequestCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcının private hesabı var mı kontrol et
            var currentUser = await _userRepository.GetByIdAsync(request.CurrentUserId);
            if (currentUser == null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            if (!currentUser.IsPrivate)
                throw new ValidationException("Sadece private hesaplar takip isteklerini onaylayabilir.");

            // Bekleyen takip isteğini bul
            var followRequest = await _followRepository.FirstOrDefaultAsync(
                f => f.FollowerId == request.RequestUserId &&
                     f.FolloweeId == request.CurrentUserId &&
                     !f.IsAccepted
            );

            if (followRequest == null)
                throw new NotFoundException("Bekleyen takip isteği bulunamadı.");

            // İsteği kabul et
            followRequest.IsAccepted = true;
            followRequest.AcceptedAt = DateTime.UtcNow;

            await _followRepository.UpdateAsync(followRequest);
            await _followRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
