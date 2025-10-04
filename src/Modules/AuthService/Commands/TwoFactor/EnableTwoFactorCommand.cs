using MediatR;

namespace AuthService.Commands.TwoFactor
{
    public record EnableTwoFactorCommand(Guid UserId, bool Enable, string IpAddress, string UserAgent) : IRequest<bool>;
}
