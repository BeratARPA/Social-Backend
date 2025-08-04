using MediatR;

namespace UserService.Queries.GetUser
{
    public record GetCurrentUserQuery(Guid UserId) : IRequest<string>;
}
