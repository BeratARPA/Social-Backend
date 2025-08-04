using MediatR;

namespace UserService.Commands.UpdateProfile
{
    public record UpdateProfileCommand(Guid UserId) : IRequest<bool>;
}
