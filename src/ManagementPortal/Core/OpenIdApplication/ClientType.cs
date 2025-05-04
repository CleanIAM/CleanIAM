using System.Text.Json.Serialization;

namespace ManagementPortal.Core.OpenIdApplication;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClientType
{
    Public,
    Confidential
}