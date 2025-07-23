using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using AuthService.Dtos;
using AuthService.Services;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.TokenRefresh
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResultDto>
    {
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(
            IGenericRepository<RefreshToken> refreshTokenRepository,
            IGenericRepository<UserCredential> userRepository,
            ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refresh = await _refreshTokenRepository.FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

            if (refresh == null || refresh.ExpiresAt < DateTime.UtcNow || refresh.IsRevoked)
                throw new UnauthorizedException("Geçersiz ya da süresi dolmuş refresh token");

            var user = await _userRepository.GetByIdAsync(refresh.UserCredentialId);
            if (user == null)
                throw new NotFoundException("Kullanıcı bulunamadı");

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            refresh.IsRevoked = true;

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserCredentialId = user.Id
            });

            await _refreshTokenRepository.UnitOfWork.SaveEntitiesAsync();

            return new AuthResultDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };
        }
    }
}
