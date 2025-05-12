using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Application.Interfaces;

namespace SharedKernel.Api.Controllers;

/// <summary>
/// Controller for maintenance endpoints.
/// </summary>
public class StatusCheckController(IAppConfiguration appConfiguration): Controller
{
    /// <summary>
    /// Get app health status
    /// </summary>
    [AllowAnonymous]
    [HttpGet("/healthz")]
    public IResult GetHealthStatus()
    {
        return Results.Ok();
    }

    /// <summary>
    /// Get app ready status
    /// </summary>
    [AllowAnonymous]
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