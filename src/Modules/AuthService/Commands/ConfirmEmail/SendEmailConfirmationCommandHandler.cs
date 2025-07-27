using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ConfirmEmail
{
    public class SendEmailConfirmationCommandHandler : IRequestHandler<SendEmailConfirmationCommand, bool>
    {
        private readonly IGenericRepository<EmailConfirmationCode> _confirmationCodeRepository;
        private readonly IGenericRepository<UserCredential> _userRepository;

        public SendEmailConfirmationCommandHandler(
            IGenericRepository<EmailConfirmationCode> confirmationCodeRepository,
            IGenericRepository<UserCredential> userRepository)
        {
            _confirmationCodeRepository = confirmationCodeRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            var code = new Random().Next(100000, 999999).ToString(); // 6 haneli
            var emailConfirmationCode = new EmailConfirmationCode
            {
                Email = request.Email,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            };

            await _confirmationCodeRepository.AddAsync(emailConfirmationCode);
            await _confirmationCodeRepository.UnitOfWork.SaveEntitiesAsync();


            return true;
        }
    }
}
