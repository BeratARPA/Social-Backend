using MediatR;

namespace UserService.Commands.Follow
{
    public record DeclineFollowRequestCommand(Guid CurrentUserId, Guid RequestUserId) : IRequest<bool>;
}
