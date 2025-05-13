using Marten;
using UrlShortner.Core;

namespace UrlShortner.Application.Queries;

public record GetShortenedUrlByIdQuery(Guid Id);


public class GetShortenedUrlByIdQueryHandler
{
    public static async Task<ShortenedUrl?> HandleAsync(GetShortenedUrlByIdQuery query, IQuerySession session, CancellationToken cancellationToken)
    {
        return await session.LoadAsync<ShortenedUrl>(query.Id, cancellationToken);
    }
}