using Mapster;
using Marten;
using SharedKernel.Infrastructure.Utils;
using UrlShortner.Core;
using UrlShortner.Core.Events;
using Wolverine;

namespace UrlShortner.Application.Commands;

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

        var baseUrl = configuration["UrlShortener:BaseUrl"];

        var urlShortened = shortenedUrl.Adapt<UrlShortened>() with { ShortenedUrl = $"{baseUrl}/{shortenedUrl.Id}" };
        await bus.PublishAsync(urlShortened);
        return Result.Ok(urlShortened);
    }
}