using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using MediatR;

namespace AuthService.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepository;

        public LogoutCommandHandler(IGenericRepository<RefreshToken> refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = await _refreshTokenRepository.FirstOrDefaultAsync(t => t.Token == request.RefreshToken);
            if (token == null)
                return false;

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = request.IpAddress;
            token.UserAgent = request.UserAgent;

           return await _refreshTokenRepository.UnitOfWork.SaveEntitiesAsync();         
        }
    }
}
