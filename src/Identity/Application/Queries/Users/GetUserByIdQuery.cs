using Marten;
using SharedKernel.Core.Users;

namespace Identity.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id);

public class GetUserByIdQueryHandler
{
    public static User? Handle(GetUserByIdQuery query, IDocumentSession session)
    {
        return session.Query<User>().FirstOrDefault(u => u.Id == query.Id);
    }
}