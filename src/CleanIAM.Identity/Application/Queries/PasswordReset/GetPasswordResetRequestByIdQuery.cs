using CleanIAM.Identity.Core.Requests;
using Marten;

namespace CleanIAM.Identity.Application.Queries.PasswordReset;

public record GetPasswordResetRequestByIdQuery(Guid Id);

public class GetPasswordResetRequestByIdQueryHandler
{
    public static async Task<PasswordResetRequest?> HandleAsync(
        GetPasswordResetRequestByIdQuery query,
        IQuerySession session,
        CancellationToken cancellationToken)
    {
        return await session.LoadAsync<PasswordResetRequest>(query.Id, cancellationToken);
    }
}