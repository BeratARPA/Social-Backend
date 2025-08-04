using MediatR;

namespace UserService.Commands.Follow
{
    public record AcceptFollowRequestCommand(Guid CurrentUserId, Guid RequestUserId) : IRequest<bool>;
}
