using AuthService.Data.Entities;
using AuthService.Data.Repositories;
using ExceptionHandling.Exceptions;
using MediatR;

namespace AuthService.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
    {
        private readonly IGenericRepository<UserCredential> _userRepository;

        public ConfirmEmailCommandHandler(IGenericRepository<UserCredential> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                throw new NotFoundException("UserNotFound");

            user.IsEmailConfirmed = true;

           await _userRepository.UpdateAsync(user);

            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
