using ManagementPortal.Core;
using Marten;

namespace ManagementPortal.Application.Queries.Users;

public record GetUserByIdQuery(Guid Id);

public class GetUserByIdQueryHandler
{
    public static async Task<User?> Handle(GetUserByIdQuery query, IQuerySession session)
    {
        return await session.LoadAsync<User>(query.Id);
    }
}