using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using AuthService.Services;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;
        private readonly ITokenService _tokenService;

        public ForgotPasswordCommandHandler(
           IGenericRepository<UserCredential> userRepository,
           ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            // 1) Validate generic token  
            var claimsPrincipal = _tokenService.ValidateToken(request.ActionToken);

            var purpose = _tokenService.FindClaimValue(claimsPrincipal, "token_type");
            if (!string.Equals(purpose, "reset_password", StringComparison.Ordinal))
                throw new ValidationException("InvalidTokenPurpose");

            var tokenEmail = _tokenService.FindClaimValue(claimsPrincipal, "target");
            if (!string.Equals(tokenEmail, request.Email, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("TokenEmailMismatch");

            // 2) Get user
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            // 3) Validate new password
            if (request.NewPassword != request.ConfirmPassword)
                throw new ValidationException("PasswordsDoNotMatch");

            // 4) Update password
            user.PasswordHash = PasswordHasher.Hash(request.NewPassword);

            await _userRepository.UpdateAsync(user);
            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
