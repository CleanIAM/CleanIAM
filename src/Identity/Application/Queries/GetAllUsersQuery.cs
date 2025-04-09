using ManagementPortal.Core;
using Marten;

namespace ManagementPortal.Application.Queries;

public record GetAllUsersQuery();

public class GetAllUsersQueryHandler
{
    public static async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, IQuerySession session)
    {
        return await session.Query<User>().ToListAsync();
    }
}