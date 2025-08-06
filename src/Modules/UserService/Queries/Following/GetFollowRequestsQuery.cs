using MediatR;
using UserService.Dtos;

namespace UserService.Queries.Following
{
    public record GetFollowRequestsQuery(Guid UserId) : IRequest<List<FollowRequestDto>>;
}
