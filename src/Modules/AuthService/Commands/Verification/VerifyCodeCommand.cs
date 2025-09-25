using AuthService.Data.Entities;
using EventBus.IntegrationEvents;
using MediatR;

namespace AuthService.Commands.Verification
{
    public record VerifyCodeCommand(VerificationChannel VerificationChannel, VerificationType VerificationType, string Target, string Code, string IpAddress, string UserAgent) : IRequest<bool>;
}
