using AuthService.Dtos;
using MediatR;

namespace AuthService.Commands.TokenRefresh
{
    public record RefreshTokenCommand(string RefreshToken, string IpAddress, string UserAgent) : IRequest<AuthResultDto>;
}
