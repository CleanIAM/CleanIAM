using Identity.Core;
using Marten;

namespace Identity.Application.Queries.PasswordReset;

public record GetPasswordResetRequestByIdQuery(Guid Id);

public class GetPasswordResetRequestByIdQueryHandler
{
    public static async Task<PasswordResetRequest?> Handle(
        GetPasswordResetRequestByIdQuery query,
        IQuerySession session,
        CancellationToken cancellationToken)
    {
        return await session.LoadAsync<PasswordResetRequest>(query.Id, cancellationToken);
    }
}