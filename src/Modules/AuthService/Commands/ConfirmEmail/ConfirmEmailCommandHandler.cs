using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IGenericRepository<ConfirmationCode> _confirmationCodeRepository;
        private readonly IEventBus _eventBus;

        public ConfirmEmailCommandHandler(
            IGenericRepository<UserCredential> userRepository,
            IGenericRepository<ConfirmationCode> confirmationCodeRepository,
            IEventBus eventBus)
        {
            _userRepository = userRepository;
            _confirmationCodeRepository = confirmationCodeRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            var confirmationCode = await _confirmationCodeRepository.FirstOrDefaultAsync(c => c.Target == request.Email && c.Code == request.Code && c.Type == ConfirmationType.Email);
            if (confirmationCode == null)
                throw new NotFoundException("ConfirmationCodeNotFound");

            if (confirmationCode.IsUsed)
                throw new ValidationException("ConfirmationCodeAlreadyUsed");

            if (confirmationCode.IsExpired)
                throw new ValidationException("ConfirmationCodeExpired");

            await _confirmationCodeRepository.DeleteAsync(confirmationCode.Id);
            await _confirmationCodeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            user.IsEmailConfirmed = true;
            await _userRepository.UpdateAsync(user);

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                _eventBus.Publish(new UserRegisteredIntegrationEvent(
                user.Id,
                user.Username
                ));

                return true;
            }

            return false;
        }
    }
}
