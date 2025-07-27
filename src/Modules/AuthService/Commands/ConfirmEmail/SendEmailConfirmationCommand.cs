using MediatR;

namespace AuthService.Commands.ConfirmEmail
{
    public record SendEmailConfirmationCommand(string Email, string IpAddress, string UserAgent) : IRequest<bool>;
}
