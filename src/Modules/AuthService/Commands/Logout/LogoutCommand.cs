using MediatR;

namespace AuthService.Commands.Logout
{
    public record LogoutCommand(string RefreshToken, string IpAddress,string UserAgent) : IRequest<bool>;
}
