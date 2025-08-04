using MediatR;

namespace UserService.Queries.GetUser
{
    public record GetUserByIdQuery(Guid UserId) : IRequest<string>;
}
