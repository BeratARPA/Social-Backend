using AuthService.Dtos;
using MediatR;

namespace AuthService.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
    {
        public Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
