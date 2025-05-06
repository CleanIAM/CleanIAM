using Identity.Core.Users;
using Marten;

namespace Identity.Application.Queries.Users;

/// <summary>
/// Query to get a user by email
/// </summary>
/// <param name="Id">Id of user to get</param>
public record GetUserByIdQuery(Guid Id);

public class GetUserByIdQueryHandler
{
    public static IdentityUser? Handle(GetUserByIdQuery query, IDocumentSession session)
    {
        return session.Query<IdentityUser>().FirstOrDefault(u => u.Id == query.Id && u.AnyTenant());
    }
}