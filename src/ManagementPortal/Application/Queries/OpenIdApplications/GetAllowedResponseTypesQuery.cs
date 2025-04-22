using ManagementPortal.Core.OpenIdApplication;
using OpenIddict.Abstractions;

namespace ManagementPortal.Application.Queries.OpenIdApplications;

public record GetAllowedResponseTypesQuery;

public class GetAllowedResponseTypesQueryHandler
{
    public static IEnumerable<ItemWithTooltip> Handle(GetAllowedResponseTypesQuery query)
    {
        return
        [
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.ResponseTypes.Code,
                Tooltip = "Authorization code flow"
            },
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.ResponseTypes.IdToken,
                Tooltip = "Implicit flow with ID token"
            },
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.ResponseTypes.Token,
                Tooltip = "Implicit flow with access token"
            },
            new ItemWithTooltip
            {
                Value = OpenIddictConstants.ResponseTypes.None,
                Tooltip = "No response type"
            }
        ];
    }
}