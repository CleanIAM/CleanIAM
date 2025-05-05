using Marten;
using SharedKernel.Core.Users;

namespace Identity.Application.Queries.Users;

/// <summary>
/// Query to get a user by email
/// </summary>
/// <param name="Email"> Email of the user </param>
public record GetUserByEmailQuery(string Email);

public class GetUserByEmailQueryHandler
{
    public static User? Handle(GetUserByEmailQuery query, IDocumentSession session)
    {
        return session.Query<User>().FirstOrDefault(u => u.AnyTenant() &&
                                                         string.Equals(u.Email, query.Email,
                                                             StringComparison.InvariantCultureIgnoreCase));
    }
}