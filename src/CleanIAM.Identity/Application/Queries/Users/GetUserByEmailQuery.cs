using CleanIAM.Identity.Core.Users;
using Marten;

namespace CleanIAM.Identity.Application.Queries.Users;

/// <summary>
/// Query to get a user by email
/// </summary>
/// <param name="Email"> Email of the user </param>
public record GetUserByEmailQuery(string Email);

public class GetUserByEmailQueryHandler
{
    public static IdentityUser? Handle(GetUserByEmailQuery query, IDocumentSession session)
    {
        return session.Query<IdentityUser>().FirstOrDefault(u => u.AnyTenant() &&
                                                                 string.Equals(u.Email, query.Email,
                                                                     StringComparison.InvariantCultureIgnoreCase));
    }
}