using MediatR;
using Microsoft.AspNetCore.Http;

namespace UserService.Commands.UpdateProfile
{
    public record UploadAvatarCommand(Guid UserId, IFormFile File) : IRequest<string>;
}
