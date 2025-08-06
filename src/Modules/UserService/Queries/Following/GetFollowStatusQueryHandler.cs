using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;
using UserService.Dtos;

namespace UserService.Queries.Following
{
    public class GetFollowStatusQueryHandler : IRequestHandler<GetFollowStatusQuery, FollowStatusDto>
    {
        private readonly IGenericRepository<Follow> _followRepository;

        public GetFollowStatusQueryHandler(IGenericRepository<Follow> followRepository)
        {
            _followRepository = followRepository;
        }

        public async Task<FollowStatusDto> Handle(GetFollowStatusQuery request, CancellationToken cancellationToken)
        {
            // Current user -> Target user takip durumu
            var currentToTarget = await _followRepository.FirstOrDefaultAsync(
                f => f.FollowerId == request.CurrentUserId && f.FolloweeId == request.TargetUserId
            );

            // Target user -> Current user takip durumu  
            var targetToCurrent = await _followRepository.FirstOrDefaultAsync(
                f => f.FollowerId == request.TargetUserId && f.FolloweeId == request.CurrentUserId
            );

            return new FollowStatusDto
            {
                IsFollowing = currentToTarget?.IsAccepted == true,
                IsFollowedBy = targetToCurrent?.IsAccepted == true,
                HasPendingRequest = currentToTarget != null && !currentToTarget.IsAccepted,
                HasIncomingRequest = targetToCurrent != null && !targetToCurrent.IsAccepted,
                FollowedAt = currentToTarget?.IsAccepted == true ? currentToTarget.AcceptedAt : null,
                FollowRequestedAt = currentToTarget?.IsAccepted == false ? currentToTarget.RequestedAt : null
            };
        }
    }
}
