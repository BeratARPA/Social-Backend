using AuthService.Dtos;
using MediatR;

namespace AuthService.Commands.TwoFactor
{
    public record VerifyTwoFactorCommand(string TwoFactorToken, string VerificationCode, string IpAddress, string UserAgent) : IRequest<AuthResultDto>;
}
