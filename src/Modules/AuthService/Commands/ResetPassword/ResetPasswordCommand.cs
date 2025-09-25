using MediatR;

namespace AuthService.Commands.ResetPassword
{
    public record ResetPasswordCommand(Guid UserId, string CurrentPassword, string NewPassword, string ConfirmPassword, string IpAddress, string UserAgent) : IRequest<bool>;
}
