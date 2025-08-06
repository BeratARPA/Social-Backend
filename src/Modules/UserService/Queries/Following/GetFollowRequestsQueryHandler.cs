using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;
using UserService.Dtos;

namespace UserService.Queries.Following
{
    public class GetFollowRequestsQueryHandler : IRequestHandler<GetFollowRequestsQuery, List<FollowRequestDto>>
    {
        private readonly IGenericRepository<Follow> _followRepository;
        private readonly IGenericRepository<UserProfile> _userRepository;

        public GetFollowRequestsQueryHandler(
            IGenericRepository<Follow> followRepository,
            IGenericRepository<UserProfile> userRepository)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
        }

        public async Task<List<FollowRequestDto>> Handle(GetFollowRequestsQuery request, CancellationToken cancellationToken)
        {
            // Kullanıcı var mı ve private hesabı var mı?
            var currentUser = await _userRepository.GetByIdAsync(request.UserId);
            if (currentUser == null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            if (!currentUser.IsPrivate)
                return new List<FollowRequestDto>(); // Private değilse takip isteği yok

            // Bekleyen takip isteklerini getir
            var followRequests = await _followRepository.GetAsync(
                filter: f => f.FolloweeId == request.UserId && !f.IsAccepted,
                includes: "Follower",
                orderBy: q => q.OrderByDescending(f => f.RequestedAt)
            );

            return followRequests.Select(f => new FollowRequestDto
            {
                UserId = f.Follower.UserId,
                Username = f.Follower.Username,
                FullName = f.Follower.FullName,
                AvatarUrl = f.Follower.AvatarUrl,
                RequestedAt = f.RequestedAt
            }).ToList();
        }
    }
}
