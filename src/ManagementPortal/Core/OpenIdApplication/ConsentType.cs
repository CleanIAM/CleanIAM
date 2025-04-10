using System.Text.Json.Serialization;

namespace ManagementPortal.Core.OpenIdApplication;

[JsonConverter(typeof(JsonStringEnumConverter<ConsentType>))]
public enum ConsentType
{
    Explicit,
    External,
    Implicit,
    Systematic
}