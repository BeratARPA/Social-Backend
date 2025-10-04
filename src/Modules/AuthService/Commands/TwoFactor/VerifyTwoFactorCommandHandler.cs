using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using AuthService.Dtos;
using AuthService.Services;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AuthService.Commands.TwoFactor
{
    public class VerifyTwoFactorCommandHandler : IRequestHandler<VerifyTwoFactorCommand, AuthResultDto>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly IGenericRepository<ConfirmationCode> _confirmationCodeRepository;
        private readonly ITokenService _tokenService;

        public VerifyTwoFactorCommandHandler(
            IGenericRepository<UserCredential> userRepository,
            IGenericRepository<RefreshToken> refreshTokenRepository,
            IGenericRepository<ConfirmationCode> confirmationCodeRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _confirmationCodeRepository = confirmationCodeRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResultDto> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Mevcut ValidateToken metodu kullanılıyor
                var claimsPrincipal = _tokenService.ValidateToken(request.TwoFactorToken);

                // Purpose kontrolü
                var purposeClaim = _tokenService.FindClaimValue(claimsPrincipal, "token_type");
                if (purposeClaim != "two_factor_auth")
                    throw new ValidationException("InvalidTwoFactorToken");

                // User ID çıkar
                var subClaim = _tokenService.FindClaimValue(claimsPrincipal, ClaimTypes.NameIdentifier);
                if (subClaim == null || !Guid.TryParse(subClaim, out var userId))
                    throw new ValidationException("InvalidTwoFactorToken");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null || !user.TwoFactorEnabled)
                    throw new ValidationException("InvalidTwoFactorToken");

                // Verification code kontrolü
                var confirmationCode = await _confirmationCodeRepository.FirstOrDefaultAsync(
                    c => c.Target == user.PhoneNumber &&
                         c.Code == request.VerificationCode &&
                         c.VerificationType == VerificationType.TwoFactor &&
                         c.VerificationChannel == VerificationChannel.WhatsApp);

                if (confirmationCode == null)
                    throw new NotFoundException("VerificationCodeNotFound");

                if (confirmationCode.IsUsed)
                    throw new ValidationException("VerificationCodeAlreadyUsed");

                if (confirmationCode.IsExpired)
                    throw new ValidationException("VerificationCodeExpired");

                // Code'u kullanıldı olarak işaretle
                confirmationCode.IsUsed = true;
                confirmationCode.UsedAt = DateTime.UtcNow;
                confirmationCode.UsedByIp = request.IpAddress;

                // Mevcut metotları kullanarak token üret
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                await _refreshTokenRepository.AddAsync(new RefreshToken
                {
                    UserCredentialId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedByIp = request.IpAddress,
                    UserAgent = request.UserAgent,
                });

                if (await _userRepository.UnitOfWork.SaveEntitiesAsync())
                {
                    return new AuthResultDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        Username = user.Username,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                    };
                }

                return new();
            }
            catch (SecurityTokenException)
            {
                throw new ValidationException("InvalidTwoFactorToken");
            }
        }
    }
}
