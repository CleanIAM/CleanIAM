using ManagementPortal.Core.Users;
using Marten;
using SharedKernel.Core.Users;

namespace ManagementPortal.Application.Queries.Users;

public record GetAllUsersCountQuery;

public class GetAllUsersCountQueryHandler
{
    public static async Task<long> Handle(GetAllUsersCountQuery query, IQuerySession session)
    {
        return await session.Query<User>().CountAsync();
    }
}