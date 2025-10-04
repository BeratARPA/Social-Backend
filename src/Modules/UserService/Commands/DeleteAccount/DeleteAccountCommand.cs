using MediatR;

namespace UserService.Commands.DeleteAccount
{
    public record DeleteAccountCommand(Guid UserId) : IRequest<bool>;
}
