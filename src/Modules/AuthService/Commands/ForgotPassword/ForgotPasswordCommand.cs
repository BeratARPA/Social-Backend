using MediatR;

namespace AuthService.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string ActionToken, string Email, string NewPassword, string ConfirmPassword, string IpAddress, string UserAgent) : IRequest<bool>;
}
