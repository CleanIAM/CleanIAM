using System.Text.Json.Serialization;

namespace ManagementPortal.Core.OpenIdApplication;

[JsonConverter(typeof(JsonStringEnumConverter<ConsentType>))]
public enum ApplicationType
{
    Native,
    Web,
}