using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;
using UserService.Dtos;

namespace UserService.Queries.Following
{
    public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, List<FollowUserDto>>
    {
        private readonly IGenericRepository<Follow> _followRepository;
        private readonly IGenericRepository<UserProfile> _userRepository;

        public GetFollowingQueryHandler(
            IGenericRepository<Follow> followRepository,
            IGenericRepository<UserProfile> userRepository)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
        }

        public async Task<List<FollowUserDto>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
        {
            // Kullanıcı var mı?
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            // Kabul edilmiş takip ettikleri getir
            var following = await _followRepository.GetAsync(
                filter: f => f.FollowerId == request.UserId && f.IsAccepted,
                includes: "Followee",
                orderBy: q => q.OrderByDescending(f => f.AcceptedAt)
            );

            return following.Select(f => new FollowUserDto
            {
                UserId = f.Followee.UserId,
                Username = f.Followee.Username,
                FullName = f.Followee.FullName,
                AvatarUrl = f.Followee.AvatarUrl,
                FollowedAt = f.AcceptedAt ?? f.RequestedAt,
                IsPrivate = f.Followee.IsPrivate
            }).ToList();
        }
    }
}
