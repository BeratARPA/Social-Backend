using MediatR;

namespace UserService.Queries.Follow
{
   public record GetFollowingQuery(Guid UserId) : IRequest<List<string>>;
}
