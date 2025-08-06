using MediatR;

namespace UserService.Commands.Following
{
    public record DeclineFollowRequestCommand(Guid CurrentUserId, Guid RequestUserId) : IRequest<bool>;
}
