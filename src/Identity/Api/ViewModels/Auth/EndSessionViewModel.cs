using Microsoft.Extensions.Primitives;

namespace Identity.Api.ViewModels.Auth;

public class EndSessionViewModel
{
    public IEnumerable<KeyValuePair<string, StringValues>> Parameters { get; set; }
}