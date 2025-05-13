using System.Text.Json.Serialization;

namespace CleanIAM.Applications.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApplicationType
{
    Native,
    Web
}