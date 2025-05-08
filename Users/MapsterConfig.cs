using Mapster;
using Users.Api.Controllers.Models;
using Users.Core;

namespace Users;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<User, ApiUserModel>.ForType()
            .Map(dest => dest.IsMFAConfigured, src => src.MfaConfig.IsMfaConfigured);
    }
}