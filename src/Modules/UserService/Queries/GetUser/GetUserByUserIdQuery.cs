using MediatR;
using UserService.Data.Entities;

namespace UserService.Queries.GetUser
{
    public record GetUserByUserIdQuery(Guid UserId) : IRequest<UserProfile>;
}
