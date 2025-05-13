using System.Text.Json.Serialization;

namespace CleanIAM.SharedKernel.Core;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    User,
    Admin,
    MasterAdmin
}