using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SharedKernel.Api.Controllers;

/// <summary>
/// Controller for maintenance endpoints.
/// </summary>
public class StatusCheckController: Controller
{
    /// <summary>
    /// Get app health status
    /// </summary>
    [AllowAnonymous]
    [HttpGet("/healthz")]
    public static IResult GetHealthStatus()
    {
        return Results.Ok();
    }

    /// <summary>
    /// Get app ready status
    /// </summary>
    [AllowAnonymous]
    [HttpGet("/readyz")]
    public static IResult GetReadyStatus()
    {
        return Results.Ok();
    }
}