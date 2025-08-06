using MediatR;
using UserService.Data.Entities;

namespace UserService.Queries.GetUser
{
    public record GetUserByIdQuery(Guid UserId) : IRequest<UserProfile>;
}
