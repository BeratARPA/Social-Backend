using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ConfirmPhone
{
    public class ConfirmPhoneCommandHandler : IRequestHandler<ConfirmPhoneCommand, bool>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IGenericRepository<ConfirmationCode> _confirmationCodeRepository;

        public ConfirmPhoneCommandHandler(IGenericRepository<UserCredential> userRepository, IGenericRepository<ConfirmationCode> confirmationCodeRepository)
        {
            _userRepository = userRepository;
            _confirmationCodeRepository = confirmationCodeRepository;
        }

        public async Task<bool> Handle(ConfirmPhoneCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            var confirmationCode = await _confirmationCodeRepository.FirstOrDefaultAsync(c => c.Target == request.PhoneNumber && c.Code == request.Code && c.Type == ConfirmationType.Phone);
            if (confirmationCode == null)
                throw new NotFoundException("ConfirmationCodeNotFound");

            if (confirmationCode.IsUsed)
                throw new ValidationException("ConfirmationCodeAlreadyUsed");

            if (confirmationCode.IsExpired)
                throw new ValidationException("ConfirmationCodeExpired");

            await _confirmationCodeRepository.DeleteAsync(confirmationCode.Id);
            await _confirmationCodeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            user.IsPhoneConfirmed = true;
            await _userRepository.UpdateAsync(user);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
