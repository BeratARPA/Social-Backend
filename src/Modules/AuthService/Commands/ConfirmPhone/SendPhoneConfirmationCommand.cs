using MediatR;

namespace AuthService.Commands.ConfirmPhone
{
    public record SendPhoneConfirmationCommand(string PhoneNumber, string IpAddress, string UserAgent) : IRequest<bool>;
}
