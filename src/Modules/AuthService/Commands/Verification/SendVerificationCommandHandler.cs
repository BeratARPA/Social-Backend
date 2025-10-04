using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.Verification
{
    public class SendVerificationCommandHandler : IRequestHandler<SendVerificationCommand, bool>
    {
        private readonly IGenericRepository<ConfirmationCode> _confirmationCodeRepository;
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IEventBus _eventBus;

        public SendVerificationCommandHandler(
            IGenericRepository<ConfirmationCode> confirmationCodeRepository,
            IGenericRepository<UserCredential> userRepository,
            IEventBus eventBus)
        {
            _confirmationCodeRepository = confirmationCodeRepository;
            _userRepository = userRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(SendVerificationCommand request, CancellationToken cancellationToken)
        {
            // Doğrulama türüne göre kullanıcı kontrolü
            await ValidateUser(request.VerificationChannel, request.Target);

            // Doğrulama kodu oluştur
            var code = new Random().Next(100000, 999999).ToString();
            var confirmationCode = new ConfirmationCode
            {
                VerificationChannel = request.VerificationChannel,
                VerificationType = request.VerificationType,
                Target = request.Target,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedByIp = request.IpAddress,
                UserAgent = request.UserAgent
            };

            await _confirmationCodeRepository.AddAsync(confirmationCode);
            if (await _confirmationCodeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // Integration Event gönderimi           
                _eventBus.Publish(new SendVerificationCodeIntegrationEvent(
                    request.VerificationChannel,
                    request.VerificationType,
                    request.Target,
                    code
                ));

                return true;
            }

            return false;
        }

        private async Task ValidateUser(VerificationChannel verificationChannel, string target)
        {
            switch (verificationChannel)
            {
                case VerificationChannel.Email:
                    var emailUser = await _userRepository.FirstOrDefaultAsync(x => x.Email == target);
                    if (emailUser == null)
                        throw new NotFoundException("UserNotFound");
                    break;

                case VerificationChannel.Sms:
                    var phoneUser = await _userRepository.FirstOrDefaultAsync(x => x.PhoneNumber == target);
                    if (phoneUser == null)
                        throw new NotFoundException("UserNotFound");
                    break;

                case VerificationChannel.WhatsApp:
                    var waUser = await _userRepository.FirstOrDefaultAsync(x => x.PhoneNumber == target);
                    if (waUser == null)
                        throw new NotFoundException("UserNotFound");
                    break;

                default:
                    throw new ValidationException("UnsupportedVerificationChannel");
            }
        }
    }
}
