using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using AuthService.Dtos;
using AuthService.Services;
using ExceptionHandling.Exceptions;
using MediatR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            IGenericRepository<UserCredential> userRepository,
            IGenericRepository<RefreshToken> refreshTokenRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                throw new ValidationException("AccountLocked");

            if (!user.IsEmailConfirmed)
                throw new ValidationException("EmailNotConfirmed");

            if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
            {
                user.FailedLoginCount++;
                if (user.FailedLoginCount >= 5)
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
                throw new ValidationException("InvalidCredentials");
            }

            // Başarılı girişte sıfırla
            user.FailedLoginCount = 0;
            user.LockoutEnd = null;

            if (user.TwoFactorEnabled)
            {
                // Mevcut GenerateToken metodu kullanılıyor
                var twoFactorClaims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                    new Claim("token_type", "two_factor_auth"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var twoFactorToken = _tokenService.GenerateToken(twoFactorClaims, TimeSpan.FromMinutes(10));

                return new AuthResultDto
                {
                    RequiresTwoFactor = true,
                    Username = user.Username,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    TwoFactorToken = twoFactorToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10)
                };
            }

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

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken))
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
    }
}
