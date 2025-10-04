using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.TwoFactor
{
    public class EnableTwoFactorCommandHandler : IRequestHandler<EnableTwoFactorCommand, bool>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;

        public EnableTwoFactorCommandHandler(
            IGenericRepository<UserCredential> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            if (!user.IsPhoneConfirmed)
                throw new ValidationException("PhoneNotConfirmed");

            user.TwoFactorEnabled = request.Enable;

            await _userRepository.UpdateAsync(user);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
