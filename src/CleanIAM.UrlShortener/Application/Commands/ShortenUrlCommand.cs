using CleanIAM.SharedKernel.Infrastructure.Utils;
using CleanIAM.UrlShortener.Core;
using CleanIAM.UrlShortener.Core.Events;
using Mapster;
using Marten;
using Wolverine;

namespace CleanIAM.UrlShortener.Application.Commands;

public record ShortenUrlCommand(string Url);

public class ShortenUrlCommandHandler
{
    public static Result Load(ShortenUrlCommand command)
    {
        if (!Uri.TryCreate(command.Url, UriKind.Absolute, out _))
            return Result.Error("Invalid url was provided");
        return Result.Ok();
    }

    public static async Task<Result<UrlShortened>> HandleAsync(ShortenUrlCommand command, Result result,
        IDocumentSession session, IMessageBus bus, IConfiguration configuration)
    {
        if (result.IsError())
            return result;

        var shortenedUrl = new ShortenedUrl
        {
            Id = Guid.NewGuid(),
            OriginalUrl = command.Url
        };

        session.Store(shortenedUrl);
        await session.SaveChangesAsync();

        var baseUrl = configuration["CleanIAM.UrlShortener:BaseUrl"];

        var urlShortened = shortenedUrl.Adapt<UrlShortened>() with { ShortenedUrl = $"{baseUrl}/{shortenedUrl.Id}" };
        await bus.PublishAsync(urlShortened);
        return Result.Ok(urlShortened);
    }
}