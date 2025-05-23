using CleanIAM.Users.Core;
using Marten;

namespace CleanIAM.Users.Application.Queries.Users;

/// <summary>
/// Query to get user by id
/// </summary>
/// <param name="Id">Id of user to get</param>
public record GetUserByIdQuery(Guid Id);

/// <summary>
/// Handler for GetUserByIdQuery
/// </summary>
public class GetUserByIdQueryHandler
{
    public static async Task<User?> HandleAsync(GetUserByIdQuery query, IQuerySession session)
    {
        return await session.LoadAsync<User>(query.Id);
    }
}