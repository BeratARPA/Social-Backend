using EventBus.IntegrationEvents;
using MediatR;

namespace AuthService.Commands.Verification
{
    public record SendVerificationCommand(VerificationChannel VerificationChannel, VerificationType VerificationType, string Target, string IpAddress, string UserAgent) : IRequest<bool>;
}
