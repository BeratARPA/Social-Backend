using MediatR;

namespace UserService.Commands.Follow
{
    public record FollowUserCommand(Guid CurrentUserId, Guid TargetUserId) : IRequest<bool>;
}
