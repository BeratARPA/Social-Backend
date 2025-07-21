using AuthService.Dtos;
using MediatR;

namespace AuthService.Commands.TokenRefresh
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResultDto>;
}
