using MediatR;

namespace UserService.Commands.Follow
{
    public class DeclineFollowRequestCommandHandler : IRequestHandler<DeclineFollowRequestCommand, bool>
    {
        public DeclineFollowRequestCommandHandler()
        {

        }

        public Task<bool> Handle(DeclineFollowRequestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
