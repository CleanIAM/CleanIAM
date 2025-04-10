using System.Text.Json;
using ManagementPortal.Core;
using Mapster;

namespace ManagementPortal;

public static class MapsterConfig
{
    public static void Configure()
    {
            
        // Configure special properties mapping for OpenIddict application objects
        TypeAdapterConfig.GlobalSettings.ForType<object, OpenIdApplication>()
            .IgnoreNullValues(true)
            .AfterMapping((src, dest) => {
                // Handle special ID property extraction from JsonElement if necessary
                if (src is Dictionary<string, JsonElement> props && props.TryGetValue("Id", out var idElement))
                {
                    dest.Id = JsonSerializer.Deserialize<Guid>(idElement.GetRawText());
                }
            });
    }
}