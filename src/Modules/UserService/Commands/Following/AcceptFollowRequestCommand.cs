using MediatR;

namespace UserService.Commands.Following
{
    public record AcceptFollowRequestCommand(Guid CurrentUserId, Guid RequestUserId) : IRequest<bool>;
}
