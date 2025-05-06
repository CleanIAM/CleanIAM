using ManagementPortal.Core.Users;
using Marten;
using SharedKernel.Core.Users;

namespace ManagementPortal.Application.Queries.Users;

public record GetAllUsersQuery;

public class GetAllUsersQueryHandler
{
    public static async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, IQuerySession session)
    {
        return await session.Query<User>().ToListAsync();
    }
}