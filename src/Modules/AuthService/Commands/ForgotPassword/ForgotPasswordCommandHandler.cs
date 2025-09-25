using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using AuthService.Services;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IGenericRepository<ConfirmationCode> _confirmationCodeRepository;

        public ForgotPasswordCommandHandler(
           IGenericRepository<UserCredential> userRepository,
           IGenericRepository<ConfirmationCode> confirmationCodeRepository)
        {
            _userRepository = userRepository;
            _confirmationCodeRepository = confirmationCodeRepository;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Email ile kullanıcıyı bul
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            // 2. Verification code'u kontrol et
            var confirmationCode = await _confirmationCodeRepository.FirstOrDefaultAsync(
                c => c.Target == request.Email &&
                     c.Code == request.Code &&
                     c.VerificationType == VerificationType.ResetPassword &&
                     c.VerificationChannel == VerificationChannel.Email);

            if (confirmationCode == null)
                throw new NotFoundException("VerificationCodeNotFound");

            if (confirmationCode.IsUsed)
                throw new ValidationException("VerificationCodeAlreadyUsed");

            if (confirmationCode.IsExpired)
                throw new ValidationException("VerificationCodeExpired");

            // 3. Şifre validasyonu
            if (request.NewPassword != request.ConfirmPassword)
                throw new ValidationException("PasswordsDoNotMatch");

            // 4. Şifreyi güncelle
            user.PasswordHash = PasswordHasher.Hash(request.NewPassword);

            // 5. Verification code'u kullanılmış olarak işaretle
            confirmationCode.IsUsed = true;
            confirmationCode.UsedAt = DateTime.UtcNow;
            confirmationCode.UsedByIp = request.IpAddress;

            await _confirmationCodeRepository.UpdateAsync(confirmationCode);
            await _confirmationCodeRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            await _userRepository.UpdateAsync(user);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
