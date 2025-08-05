using MediatR;

namespace UserService.Commands.Follow
{
    public class AcceptFollowRequestCommandHandler : IRequestHandler<AcceptFollowRequestCommand, bool>
    {
        public AcceptFollowRequestCommandHandler()
        {

        }

        public Task<bool> Handle(AcceptFollowRequestCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
