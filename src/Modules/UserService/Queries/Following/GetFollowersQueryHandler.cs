using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;
using UserService.Dtos;

namespace UserService.Queries.Following
{
    public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, List<FollowUserDto>>
    {
        private readonly IGenericRepository<Follow> _followRepository;
        private readonly IGenericRepository<UserProfile> _userRepository;

        public GetFollowersQueryHandler(
            IGenericRepository<Follow> followRepository,
            IGenericRepository<UserProfile> userRepository)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
        }

        public async Task<List<FollowUserDto>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
        {
            // Hedef kullanıcı var mı?
            var targetUser = await _userRepository.GetByIdAsync(request.UserId);
            if (targetUser == null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            // Kabul edilmiş takipçileri getir
            var followers = await _followRepository.GetAsync(
                filter: f => f.FolloweeId == request.UserId && f.IsAccepted,
                includes: "Follower",
                orderBy: q => q.OrderByDescending(f => f.AcceptedAt)
            );

            return followers.Select(f => new FollowUserDto
            {
                UserId = f.Follower.UserId,
                Username = f.Follower.Username,
                FullName = f.Follower.FullName,
                AvatarUrl = f.Follower.AvatarUrl,
                FollowedAt = f.AcceptedAt ?? f.RequestedAt,
                IsPrivate = f.Follower.IsPrivate
            }).ToList();
        }
    }
}
