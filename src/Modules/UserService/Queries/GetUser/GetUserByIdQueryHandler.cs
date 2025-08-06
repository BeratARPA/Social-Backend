using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Queries.GetUser
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserProfile>
    {
        private readonly IGenericRepository<UserProfile> _userProfileRepository;

        public GetUserByIdQueryHandler(IGenericRepository<UserProfile> userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public async Task<UserProfile> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            return user;
        }
    }
}
