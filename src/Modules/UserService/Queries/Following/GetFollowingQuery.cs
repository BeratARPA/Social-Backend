using MediatR;
using UserService.Dtos;

namespace UserService.Queries.Following
{
    public record GetFollowingQuery(Guid UserId) : IRequest<List<FollowUserDto>>;
}
