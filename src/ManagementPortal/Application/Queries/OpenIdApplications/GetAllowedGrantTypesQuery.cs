using ManagementPortal.Core.OpenIdApplication;
using OpenIddict.Abstractions;

namespace ManagementPortal.Application.Queries.OpenIdApplications;

public record GetAllowedGrantTypesQuery;

public class GetAllowedGrantTypesQueryHandler
{
    public static IEnumerable<ItemWithTooltip> Handle(GetAllowedGrantTypesQuery query)
    {
        return
        [
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.GrantTypes.AuthorizationCode,
                Tooltip = "Authorization code flow"
            },
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.GrantTypes.ClientCredentials,
                Tooltip = "Client credentials flow"
            },
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.GrantTypes.DeviceCode,
                Tooltip = "Device code flow"
            },
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.GrantTypes.Implicit,
                Tooltip = "Implicit flow"
            },
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.GrantTypes.Password,
                Tooltip = "Resource owner password credentials flow"
            },
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.GrantTypes.RefreshToken,
                Tooltip = "Refresh token flow"
            }
        ];
    }
};