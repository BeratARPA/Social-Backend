using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.Verification
{
    public class VerifyCodeCommandHandler : IRequestHandler<VerifyCodeCommand, bool>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IGenericRepository<ConfirmationCode> _confirmationCodeRepository;
        private readonly IEventBus _eventBus;

        public VerifyCodeCommandHandler(
            IGenericRepository<UserCredential> userRepository,
            IGenericRepository<ConfirmationCode> confirmationCodeRepository,
            IEventBus eventBus)
        {
            _userRepository = userRepository;
            _confirmationCodeRepository = confirmationCodeRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcıyı bul
            var user = await FindUser(request.VerificationChannel, request.Target);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            // Doğrulama kodunu bul
            var confirmationCode = await _confirmationCodeRepository.FirstOrDefaultAsync(
                c => c.Target == request.Target &&
                     c.Code == request.Code &&
                     c.VerificationType == request.VerificationType &&
                     c.VerificationChannel == request.VerificationChannel);

            if (confirmationCode == null)
                throw new NotFoundException("VerificationCodeNotFound");

            if (confirmationCode.IsUsed)
                throw new ValidationException("VerificationCodeAlreadyUsed");

            if (confirmationCode.IsExpired)
                throw new ValidationException("VerificationCodeExpired");

            confirmationCode.IsUsed = true;
            confirmationCode.UsedAt = DateTime.UtcNow;
            confirmationCode.UsedByIp = request.IpAddress;

            await _confirmationCodeRepository.UpdateAsync(confirmationCode);
            await _confirmationCodeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            // Kullanıcı durumunu güncelle
            await UpdateUserVerificationStatus(user, request.VerificationType);
            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
            {
                // Başarılı doğrulama event'i gönder
                PublishVerificationSuccessEvent(user, request.VerificationType);

                return true;
            }

            return false;
        }

        private async Task<UserCredential?> FindUser(VerificationChannel verificationChannel, string target)
        {
            return verificationChannel switch
            {
                VerificationChannel.Email => await _userRepository.FirstOrDefaultAsync(u => u.Email == target),
                VerificationChannel.Sms => await _userRepository.FirstOrDefaultAsync(u => u.PhoneNumber == target),
                _ => null
            };
        }

        private async Task UpdateUserVerificationStatus(UserCredential user, VerificationType type)
        {
            switch (type)
            {
                case VerificationType.VerifyEmail:
                    user.IsEmailConfirmed = true;
                    break;

                case VerificationType.VerifyPhone:
                    user.IsPhoneConfirmed = true;
                    break;
            }

            await _userRepository.UpdateAsync(user);
        }

        private void PublishVerificationSuccessEvent(UserCredential user, VerificationType type)
        {
            switch (type)
            {
                case VerificationType.VerifyEmail:
                    if (user.IsEmailConfirmed)
                    {
                        _eventBus.Publish(new UserRegisteredIntegrationEvent(
                            user.Id,
                            user.Username
                        ));
                    }
                    break;

                case VerificationType.VerifyPhone:
                    //_eventBus.Publish(new PhoneVerificationCompletedIntegrationEvent(
                    //    user.Id,
                    //    user.PhoneNumber
                    //));
                    break;
            }

        }
    }
}
