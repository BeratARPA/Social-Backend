using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Queries.GetUser
{
    public class GetUserByUserIdQueryHandler : IRequestHandler<GetUserByUserIdQuery, UserProfile>
    {
        private readonly IGenericRepository<UserProfile> _userProfileRepository;

        public GetUserByUserIdQueryHandler(IGenericRepository<UserProfile> userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public async Task<UserProfile> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.FirstOrDefaultAsync(x => x.UserId == request.UserId);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            return user;
        }
    }
}
