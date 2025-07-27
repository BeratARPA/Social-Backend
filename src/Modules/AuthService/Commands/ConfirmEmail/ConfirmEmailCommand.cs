using MediatR;

namespace AuthService.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string Email, string Code, string IpAddress, string UserAgent) : IRequest<bool>;
}
