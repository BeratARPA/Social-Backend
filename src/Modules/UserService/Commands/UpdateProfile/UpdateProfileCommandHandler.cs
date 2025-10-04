using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly IGenericRepository<UserProfile> _userProfileRepository;

        public UpdateProfileCommandHandler(IGenericRepository<UserProfile> userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(request.UserId);
            if (userProfile == null)
                throw new NotFoundException("UserNotFound");

            userProfile.Username = request.dto.Username;
            userProfile.FullName = request.dto.FullName;
            userProfile.Bio = request.dto.Bio;
            userProfile.IsPrivate = request.dto.IsPrivate;

            await _userProfileRepository.UpdateAsync(userProfile);
            return await _userProfileRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
