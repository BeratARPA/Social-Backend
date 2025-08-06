using MediatR;

namespace UserService.Commands.Following
{
    public record FollowUserCommand(Guid CurrentUserId, Guid TargetUserId) : IRequest<bool>;
}
