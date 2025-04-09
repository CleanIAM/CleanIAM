using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SharedKernel.Infrastructure.Conventions;

public class KebabCaseRouteConvention //: IPageRouteModelConvention
{
    // public void Apply(PageRouteModel model)
    // {
    //     var routeTemplate = model.RouteValues;
    //     var kebabRoute = ConvertToKebabCase(routeTemplate);
    //     model.RouteTemplate = kebabRoute;
    // }
    //
    // private string ConvertToKebabCase(string input)
    // {
    //     // Convert PascalCase to kebab-case
    //     return Regex.Replace(input, "([a-z])([A-Z])", "$1-$2").ToLower();
    // }
}