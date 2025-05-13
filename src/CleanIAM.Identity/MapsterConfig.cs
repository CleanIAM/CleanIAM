using System.Reflection;
using Mapster;

namespace CleanIAM.Identity;

public class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        // Add custom mappings here
        // TypeAdapterConfig.GlobalSettings.ForType<Source, Destination>().Map(dest => dest.Property, src => src.Property);
    }
}