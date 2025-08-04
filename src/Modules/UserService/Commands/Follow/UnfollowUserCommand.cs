using MediatR;

namespace UserService.Commands.Follow
{
    public record UnfollowUserCommand(Guid CurrentUserId, Guid TargetUserId) : IRequest<bool>;
}
