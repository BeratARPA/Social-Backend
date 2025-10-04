using MediatR;

namespace UserService.Queries.SearchUsers
{
    public record SearchUsersQuery(string Query) : IRequest<List<string>>;
}
