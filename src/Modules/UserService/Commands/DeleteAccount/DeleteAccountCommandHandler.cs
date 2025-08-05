using EventBus.Base.Abstraction;
using EventBus.IntegrationEvents;
using ExceptionHandling.Exceptions;
using MediatR;
using UserService.Data.Entities;
using UserService.Data.Repositories;

namespace UserService.Commands.DeleteAccount
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
    {
        private readonly IGenericRepository<UserProfile> _userProfileRepository;
        private readonly IEventBus _eventBus;

        public DeleteAccountCommandHandler(
            IGenericRepository<UserProfile> userProfileRepository,
            IEventBus eventBus)
        {
            _userProfileRepository = userProfileRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.GetByIdAsync(request.UserId);
            if (user is null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            await _userProfileRepository.DeleteAsync(user.Id);
            await _userProfileRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            _eventBus.Publish(new UserDeletedIntegrationEvent(request.UserId));

            return true;
        }
    }
}
