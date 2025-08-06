using MediatR;

namespace UserService.Commands.Following
{
    public record UnfollowUserCommand(Guid CurrentUserId, Guid TargetUserId) : IRequest<bool>;
}
