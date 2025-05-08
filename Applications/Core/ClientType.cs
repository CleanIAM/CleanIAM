using System.Text.Json.Serialization;

namespace Applications.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClientType
{
    Public,
    Confidential
}