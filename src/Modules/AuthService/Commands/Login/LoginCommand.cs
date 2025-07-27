using AuthService.Dtos;
using MediatR;

namespace AuthService.Commands.Login
{
    public record LoginCommand(string Username, string Password, string IpAddress, string UserAgent) : IRequest<AuthResultDto>;
}
