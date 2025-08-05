using MediatR;

namespace UserService.Commands.Follow
{
    public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, bool>
    {
        public FollowUserCommandHandler()
        {

        }

        public Task<bool> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
