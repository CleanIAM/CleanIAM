using Marten;
using SharedKernel.Core.Users;

namespace Identity.Application.Queries.Users;

public record GetUserByEmailQuery(string Email);

public class GetUserByEmailQueryHandler
{
    public static User? Handle(GetUserByEmailQuery query, IDocumentSession session)
    {
        return session.Query<User>().FirstOrDefault(x =>
            string.Equals(x.Email, query.Email, StringComparison.InvariantCultureIgnoreCase));
    }
}