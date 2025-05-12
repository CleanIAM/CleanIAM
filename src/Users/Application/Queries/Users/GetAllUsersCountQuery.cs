using Marten;
using Users.Core;

namespace Users.Application.Queries.Users;

public record GetAllUsersCountQuery;

public class GetAllUsersCountQueryHandler
{
    public static async Task<long> HandleAsync(GetAllUsersCountQuery query, IQuerySession session)
    {
        return await session.Query<User>().CountAsync();
    }
}