using System.Text.Json.Serialization;

namespace ManagementPortal.Core.OpenIdApplication;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApplicationType
{
    Native,
    Web
}