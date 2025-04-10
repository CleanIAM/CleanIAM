using System.Text.Json.Serialization;

namespace ManagementPortal.Core.OpenIdApplication;

[JsonConverter(typeof(JsonStringEnumConverter<ClientType>))]
public enum ClientType
{
    Public,
    Confidential
}