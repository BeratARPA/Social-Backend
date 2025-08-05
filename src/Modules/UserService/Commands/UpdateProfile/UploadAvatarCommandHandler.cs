using MediatR;

namespace UserService.Commands.UpdateProfile
{
    public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, string>
    {
        public UploadAvatarCommandHandler()
        {

        }

        public Task<string> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
