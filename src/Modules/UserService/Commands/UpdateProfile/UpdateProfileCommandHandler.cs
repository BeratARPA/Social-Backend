using MediatR;

namespace UserService.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        public UpdateProfileCommandHandler()
        {

        }

        public Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
