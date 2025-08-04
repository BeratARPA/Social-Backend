using MediatR;

namespace UserService.Queries.Follow
{
  public record GetFollowRequestsQuery(Guid UserId) : IRequest<List<string>>;
}
