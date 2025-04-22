using ManagementPortal.Core.OpenIdApplication;
using OpenIddict.Abstractions;

namespace ManagementPortal.Application.Queries.OpenIdApplications;

public record GetAllowedEndpointsQuery;

public class GetAllowedEndpointsQueryHandler
{
    public static IEnumerable<ItemWithTooltip> Handle(GetAllowedEndpointsQuery query)
    {
        return
        [
            new()
            {
                Value = OpenIddictConstants.Permissions.Endpoints.Authorization,
                Tooltip = "Endpoint for authorization code flow"
            },
            new()
            {
                Value = OpenIddictConstants.Permissions.Endpoints.DeviceAuthorization,
                Tooltip = "Endpoint for device authorization flow"
            },
            new()
            {
                Value = OpenIddictConstants.Permissions.Endpoints.EndSession,
                Tooltip = "Endpoint for end session flow"
            },
            new()
            {
                Value = OpenIddictConstants.Permissions.Endpoints.Introspection,
                Tooltip = "Endpoint for introspection flow"
            },
            new()
            {
                Value = OpenIddictConstants.Permissions.Endpoints.PushedAuthorization,
                Tooltip = "Endpoint for pushed authorization request flow"
            },
            new()
            {
                Value = OpenIddictConstants.Permissions.Endpoints.Revocation,
                Tooltip = "Endpoint for revocation flow"
            },
            new()
            {
                Value = OpenIddictConstants.Permissions.Endpoints.Token,
                Tooltip = "Endpoint for token flow"
            }
        ];
    }
}