using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Primitives;

namespace CleanIAM.Identity.Api.ViewModels.Auth;

public class EndSessionViewModel
{
    /// <summary>
    /// Indicates if the user should be sign out also from the CleanIAM.
    /// </summary>
    [Required]
    public bool FullLogout { get; set; }

    /// <summary>
    /// Parameters to be passed to the end session endpoint.
    /// </summary>
    public IEnumerable<KeyValuePair<string, StringValues>> Parameters { get; set; }
}