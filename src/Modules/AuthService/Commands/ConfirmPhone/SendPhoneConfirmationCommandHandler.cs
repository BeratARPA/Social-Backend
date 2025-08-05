using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ConfirmPhone
{
    public class SendPhoneConfirmationCommandHandler : IRequestHandler<SendPhoneConfirmationCommand, bool>
    {
        private readonly IGenericRepository<ConfirmationCode> _confirmationCodeRepository;
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IEventBus _eventBus;

        public SendPhoneConfirmationCommandHandler(
            IGenericRepository<ConfirmationCode> confirmationCodeRepository,
            IGenericRepository<UserCredential> userRepository,
            IEventBus eventBus)
        {
            _confirmationCodeRepository = confirmationCodeRepository;
            _userRepository = userRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(SendPhoneConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            var code = new Random().Next(100000, 999999).ToString();
            var confirmationCode = new ConfirmationCode
            {
                Type = ConfirmationType.Phone,
                Target = request.PhoneNumber,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedByIp = request.IpAddress,
                UserAgent = request.UserAgent
            };

            await _confirmationCodeRepository.AddAsync(confirmationCode);
            await _confirmationCodeRepository.UnitOfWork.SaveEntitiesAsync();

            // Integration Event gönderimi
            _eventBus.Publish(new SendVerificationCodeIntegrationEvent(
               VerificationChannel.Sms,
               request.PhoneNumber,
               code
           ));

            return true;
        }
    }
}
