using MediatR;

namespace UserService.Commands.Follow
{
    public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, bool>
    {
        public UnfollowUserCommandHandler()
        {

        }

        public Task<bool> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
