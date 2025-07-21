using MediatR;

namespace AuthService.Commands.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<bool>;
}
