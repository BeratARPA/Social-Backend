using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using AuthService.Dtos;
using AuthService.Services;
using EventBus.Base.Abstraction;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResultDto>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IEventBus _eventBus;

        public RegisterCommandHandler(
            IGenericRepository<UserCredential> userRepository,
            IGenericRepository<RefreshToken> refreshTokenRepository,
            ITokenService tokenService,
            IEventBus eventBus)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _eventBus = eventBus;
        }

        public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var exists = await _userRepository.ExistsAsync(x => x.Username == request.Username);
            if (exists)
                throw new ValidationException("UsernameAlreadyExists");

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

            await _userRepository.UnitOfWork.SaveEntitiesAsync();

            return new AuthResultDto
            {
                AccessToken = _tokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken,
                Username = user.Username,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };
        }
    }
}
