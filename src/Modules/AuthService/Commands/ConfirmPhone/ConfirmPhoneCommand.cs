using MediatR;

namespace AuthService.Commands.ConfirmPhone
{
    public record ConfirmPhoneCommand(string PhoneNumber, string Code, string IpAddress, string UserAgent) : IRequest<bool>;
}
