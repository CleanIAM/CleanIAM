using System.Text.Json.Serialization;

namespace SharedKernel.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    User,
    Admin,
    MasterAdmin
}