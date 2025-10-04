using MediatR;
using UserService.Dtos;

namespace UserService.Commands.UpdateProfile
{
    public record UpdateProfileCommand(Guid UserId, UpdateProfileRequestDto dto) : IRequest<bool>;
}
