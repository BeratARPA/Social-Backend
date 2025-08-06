using MediatR;
using UserService.Dtos;

namespace UserService.Queries.Following
{
    public record GetFollowersQuery(Guid UserId) : IRequest<List<FollowUserDto>>;
}
