using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ConfirmEmail
{
    public class SendEmailConfirmationCommandHandler : IRequestHandler<SendEmailConfirmationCommand, bool>
    {
        private readonly IGenericRepository<ConfirmationCode> _confirmationCodeRepository;
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IEventBus _eventBus;

        public SendEmailConfirmationCommandHandler(
            IGenericRepository<ConfirmationCode> confirmationCodeRepository,
            IGenericRepository<UserCredential> userRepository,
            IEventBus eventBus)
        {
            _confirmationCodeRepository = confirmationCodeRepository;
            _userRepository = userRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            var code = new Random().Next(100000, 999999).ToString();
            var confirmationCode = new ConfirmationCode
            {
                Type = ConfirmationType.Email,
                Target = request.Email,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedByIp = request.IpAddress,
                UserAgent = request.UserAgent
            };

            await _confirmationCodeRepository.AddAsync(confirmationCode);
            await _confirmationCodeRepository.UnitOfWork.SaveEntitiesAsync();

            // Integration Event gönderimi
             _eventBus.Publish(new SendVerificationCodeIntegrationEvent(
                VerificationChannel.Email,
                request.Email,
                code
            ));

            return true;
        }
    }
}
