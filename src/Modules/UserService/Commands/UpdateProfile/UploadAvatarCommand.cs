using MediatR;

namespace UserService.Commands.UpdateProfile
{
    public record UploadAvatarCommand(Guid UserId) : IRequest<string>;
}
