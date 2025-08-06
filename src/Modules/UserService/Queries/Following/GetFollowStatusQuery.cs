using MediatR;
using UserService.Dtos;

namespace UserService.Queries.Following
{
    public record GetFollowStatusQuery(Guid CurrentUserId, Guid TargetUserId) : IRequest<FollowStatusDto>;
}
