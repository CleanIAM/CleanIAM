using CleanIAM.Users.Core;
using Marten;

namespace CleanIAM.Users.Application.Queries.Users;

public record GetAllUsersQuery;

public class GetAllUsersQueryHandler
{
    public static async Task<IEnumerable<User>> HandleAsync(GetAllUsersQuery query, IQuerySession session)
    {
        return await session.Query<User>().ToListAsync();
    }
}