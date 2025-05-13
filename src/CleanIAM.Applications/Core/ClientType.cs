using System.Text.Json.Serialization;

namespace CleanIAM.Applications.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ClientType
{
    Public,
    Confidential
}