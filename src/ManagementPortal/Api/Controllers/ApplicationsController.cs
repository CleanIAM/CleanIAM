using ManagementPortal.Api.Views.Applications;
using ManagementPortal.Application.Commands.OpenIdApplications;
using ManagementPortal.Application.Queries.OpenIdApplications;
using ManagementPortal.Core.Events.OpenIdApplications;
using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using Marten;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

[Route("applications")]
public class ApplicationsController(
    IMessageBus bus) : Controller
{

    /// <summary>
    /// Show the main application page with a list of all applications.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        var query = new GetAllOpenIdApplicationsQuery();
        var applications = await bus.InvokeAsync<IEnumerable<OpenIdApplication>>(query);

        return View(new ApplicationPageModel { Applications = applications.ToArray() });
    }

    /// <summary>
    /// Get form for creating new oidc application
    /// </summary>
    /// <returns></returns>
    [HttpGet("new")]
    public IActionResult CreateNewApplicationAsync()
    {
        return View();
    }

    /// <summary>
    /// Handle form submission for creating new oidc application
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("new")]
    public async Task<IActionResult> CreateNewApplicationAsync([FromForm] CreateNewApplicationModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var command = model.Adapt<CreateNewOpenIdApplicationCommand>() with { Id = Guid.NewGuid() };
        var res = await bus.InvokeAsync<Result<OpenIdApplicationCreated>>(command, cancellationToken);

        if (res.IsError())
        {
            ModelState.AddModelError("asdf", res.ErrorValue.Message);
            return View(model);
        }

        var applicationCreatedModel = new ApplicationCreatedPopup
        {
            ClientId = res.Value.ClientId,
            ClientType = res.Value.ClientType ?? ClientType.Public,
            ClientSecret = res.Value.ClientSecret
        };
        
        // For modal popup approach - using TempData since we're redirecting
        TempData["ShowApplicationCreatedModal"] = true;
        TempData["ApplicationCreatedClientId"] = applicationCreatedModel.ClientId;
        TempData["ApplicationCreatedClientType"] = applicationCreatedModel.ClientType;
        
        // Only set the client secret in TempData if it's not null or empty
        if (!string.IsNullOrEmpty(applicationCreatedModel.ClientSecret))
        {
            TempData["ApplicationCreatedClientSecret"] = applicationCreatedModel.ClientSecret;
        }
        
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Show the edit page for a specific application.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> EditApplicationAsync([FromRoute] Guid id)
    {
        var query = new GetOpenIdApplicationByIdQuery(id);
        var application = await bus.InvokeAsync<OpenIdApplication?>(query);

        if (application is null)
            return NotFound();

        var model = application.Adapt<EditApplicationModel>();

        return View(model);
    }

    /// <summary>
    /// Update the application with the given id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> EditPostAsync([FromRoute] Guid id, EditApplicationModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("EditApplication", model);
        }

        var command = model.Adapt<UpdateOpenIdApplicationCommand>();
        var result = await bus.InvokeAsync<Result<OpenIdApplicationUpdated>>(command);
        if (result.IsError())
        {
            ModelState.AddModelError("", result.ErrorValue.Message);
            return View("EditApplication", model);
        }

        return RedirectToAction("Index");
    }

    /// <summary>
    /// HTMX endpoint to delete an application.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteApplication([FromRoute] Guid id)
    {
        var command = new DeleteOpenIdApplicationCommand(id);
        var result = await bus.InvokeAsync<Result<OpenIdApplicationDeleted>>(command);
        if (result.IsError())
        {
            return BadRequest(result.ErrorValue.Message);
        }

        return Ok();
    }

    /// <summary>
    /// HTMX endpoint to add a new URI Item to the post logout uri list.
    /// </summary>
    /// <param name="id">application id</param>
    /// <param name="postLogoutUri">uri to include into generated html</param>
    /// <param name="type">Type of the uri (Application.PostLogoutRedirectUris/)</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit/post-logout-uri")]
    public IActionResult AddPostLogout([FromRoute] Guid id, [FromForm] string postLogoutUri, [FromQuery] string type)
    {
        if (string.IsNullOrWhiteSpace(postLogoutUri))
        {
            return BadRequest("URI cannot be empty");
        }

        if (!Uri.TryCreate(postLogoutUri, UriKind.Absolute, out var parsedUri))
        {
            return BadRequest("Invalid URI format.");
        }

        // Return the partial view for HTMX to add to the DOM
        return View("_DynamicListItem",
            new DynamicListItem { Value = parsedUri.ToString(), Id = id, Name = "PostLogoutRedirectUris" });
    }

    /// <summary>
    /// HTMX endpoint to add a new URI Item to the redirect uri list.
    /// </summary>
    /// <param name="id">application id</param>
    /// <param name="redirectUri">uri to include into generated html</param>
    /// <param name="type">Type of the uri (Application.PostLogoutRedirectUris/)</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit/redirect-uri")]
    public IActionResult AddRedirectUri([FromRoute] Guid id, [FromForm] string redirectUri, [FromQuery] string type)
    {
        if (string.IsNullOrWhiteSpace(redirectUri))
        {
            return BadRequest("URI cannot be empty");
        }

        if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var parsedUri))
        {
            return BadRequest("Invalid URI format.");
        }

        // Return the partial view for HTMX to add to the DOM
        return View("_DynamicListItem",
            new DynamicListItem { Value = parsedUri.ToString(), Id = id, Name = "RedirectUris" });
    }

    /// <summary>
    /// HTMX endpoint to add a new permission item to the list.
    /// </summary>
    /// <param name="id">application id</param>
    /// <param name="permission">permission to include into generated html</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit/permission")]
    public IActionResult AddPermission([FromRoute] Guid id, [FromForm] string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return BadRequest("permission cannot be empty");
        }

        // TODO: Validate the permission format

        // Return the partial view for HTMX to add to the DOM
        return View("_DynamicListItem", new DynamicListItem { Value = permission, Id = id, Name = "Permissions" });
    }
}