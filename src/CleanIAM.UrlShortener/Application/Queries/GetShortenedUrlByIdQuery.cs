using CleanIAM.UrlShortener.Core;
using Marten;

namespace CleanIAM.UrlShortener.Application.Queries;

public record GetShortenedUrlByIdQuery(Guid Id);


public class GetShortenedUrlByIdQueryHandler
{
    public static async Task<ShortenedUrl?> HandleAsync(GetShortenedUrlByIdQuery query, IQuerySession session, CancellationToken cancellationToken)
    {
        return await session.LoadAsync<ShortenedUrl>(query.Id, cancellationToken);
    }
}