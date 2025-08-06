using MediatR;
using UserService.Data.Entities;

namespace UserService.Queries.GetUser
{
    public record GetCurrentUserQuery(Guid UserId) : IRequest<UserProfile>;
}
