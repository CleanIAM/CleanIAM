namespace UrlShortner.Core.Events;

public record UrlShortened(Guid Id, string OriginalUrl, string ShortenedUrl);