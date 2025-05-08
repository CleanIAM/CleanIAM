using System.Text.Json;
using Mapster;
using OpenIddict.EntityFrameworkCore.Models;
using Scopes.Application.Commands;
using Scopes.Core;

namespace Scopes;

public class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<CreateNewScopeCommand, OpenIddictEntityFrameworkCoreScope<Guid>>.ForType()
            .AfterMapping((src, dest) => { dest.Resources = JsonSerializer.Serialize(src.Resources); });

        TypeAdapterConfig<UpdateScopeCommand, OpenIddictEntityFrameworkCoreScope<Guid>>.ForType()
            .AfterMapping((src, dest) => { dest.Resources = JsonSerializer.Serialize(src.Resources); });

        TypeAdapterConfig<OpenIddictEntityFrameworkCoreScope<Guid>, Scope>.ForType()
            .Ignore(dest => dest.Resources)
            .AfterMapping((src, dest) =>
            {
                var resouce = src.Resources ?? "[]";
                var json = JsonSerializer.Deserialize<string[]>(resouce);
                dest.Resources = json ?? [];
            });
    }
}