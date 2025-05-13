using CleanIAM.Users.Api.Controllers.Models;
using CleanIAM.Users.Core;
using Mapster;

namespace CleanIAM.Users;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<User, ApiUserModel>.ForType()
            .Map(dest => dest.IsMFAConfigured, src => src.MfaConfig.IsMfaConfigured);
    }
}