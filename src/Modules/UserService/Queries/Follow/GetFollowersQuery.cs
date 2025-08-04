using MediatR;

namespace UserService.Queries.Follow
{
   public record GetFollowersQuery(Guid UserId) : IRequest<List<string>>;
}
