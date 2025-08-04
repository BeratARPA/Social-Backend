using MediatR;

namespace UserService.Queries.GetSuggestions
{
   public record GetSuggestionsQuery(Guid UserId) : IRequest<List<string>>;
}
