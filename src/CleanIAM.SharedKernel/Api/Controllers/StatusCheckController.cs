using CleanIAM.SharedKernel.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanIAM.SharedKernel.Api.Controllers;

/// <summary>
/// Controller for maintenance endpoints.
/// </summary>
public class StatusCheckController(IAppConfiguration appConfiguration): Controller
{
    /// <summary>
    /// Get app health status
    /// </summary>
    [HttpGet("/healthz")]
    public IResult GetHealthStatus()
    {
        return Results.Ok();
    }

    /// <summary>
    /// Get app ready status
    /// </summary>
    [HttpGet("/readyz")]
    public IResult GetReadyStatus()
    {
        return Results.Ok();
    }
    
    /// <summary>
    /// Redirect to the management portal.
    /// </summary>
    [HttpGet("/")]
    public IActionResult Index()
    {
        return Redirect(appConfiguration.ManagementPortalBaseUrl);
    }
}