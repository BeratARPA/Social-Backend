using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using AuthService.Services;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;

        public ResetPasswordCommandHandler(
            IGenericRepository<UserCredential> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            var verificationResult = PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash);
            if (verificationResult)
                throw new ValidationException("CurrentPasswordIncorrect");

            if (request.NewPassword != request.ConfirmPassword)
                throw new ValidationException("PasswordsDoNotMatch");

            user.PasswordHash = PasswordHasher.Hash(request.NewPassword);

            await _userRepository.UpdateAsync(user);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
