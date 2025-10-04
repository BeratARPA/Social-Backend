using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using AuthService.Dtos;
using AuthService.Services;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly ITokenService _tokenService;

        public RegisterCommandHandler(
            IGenericRepository<UserCredential> userRepository,
            IGenericRepository<RefreshToken> refreshTokenRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var usernameExists = await _userRepository.ExistsAsync(x => x.Username == request.Username);
            if (usernameExists)
                throw new ValidationException("UsernameAlreadyExists");

            var emailExists = await _userRepository.ExistsAsync(x => x.Email == request.Email);
            if (emailExists)
                throw new ValidationException("EmailAlreadyExists");

            var user = new UserCredential
            {
                Username = request.Username,
                PasswordHash = PasswordHasher.Hash(request.Password),
                Email = request.Email
            };

            await _userRepository.AddAsync(user);

            var refreshToken = _tokenService.GenerateRefreshToken();
            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserCredential = user,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = request.IpAddress,
                UserAgent = request.UserAgent,
            });

            if (await _userRepository.UnitOfWork.SaveEntitiesAsync())
            {
                return new AuthResultDto
                {
                    AccessToken = _tokenService.GenerateAccessToken(user),
                    RefreshToken = refreshToken,
                    Username = user.Username,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                };
            }

            return new();
        }
    }
}
