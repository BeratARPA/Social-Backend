using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.Following
{
    public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, bool>
    {
        private readonly IGenericRepository<Follow> _followRepository;

        public UnfollowUserCommandHandler(IGenericRepository<Follow> followRepository)
        {
            _followRepository = followRepository;
        }

        public async Task<bool> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            // Follow kaydını bul
            var follow = await _followRepository.FirstOrDefaultAsync(
                f => f.FollowerId == request.CurrentUserId && f.FolloweeId == request.TargetUserId
            );

            if (follow == null)
                throw new NotFoundException("Takip kaydı bulunamadı.");

            // Takibi sonlandır (soft delete)
            await _followRepository.SoftDeleteAsync(request.CurrentUserId);
            await _followRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
