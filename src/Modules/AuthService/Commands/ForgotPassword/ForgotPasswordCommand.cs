using MediatR;

namespace AuthService.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email, string NewPassword, string ConfirmPassword, string Code, string IpAddress, string UserAgent) : IRequest<bool>;
}
