using MediatR;

namespace UserService.Commands.RestoreAccount
{
    public record RestoreAccountCommand(Guid UserId) : IRequest<bool>;
}
