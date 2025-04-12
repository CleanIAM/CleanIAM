using ManagementPortal.Api.Views.Applications;
using ManagementPortal.Api.Views.Applications.Edit;
using ManagementPortal.Application.Commands.OpenIdApplications;
using ManagementPortal.Application.Queries.OpenIdApplications;
using ManagementPortal.Core.OpenIdApplication;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Api.Controllers;

[Route("applications")]
public class ApplicationsController(
    IMessageBus bus
    ): Controller
{
    
    /// <summary>
    /// Show the main application page with a list of all applications.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        var query = new GetAllOpenIdApplicationsQuery();
        var applications = await bus.InvokeAsync<IEnumerable<OpenIdApplication>>(query);
        
        return View(new ApplicationPageModel {Applications = applications.ToArray()});
    }
    
    /// <summary>
    /// Show the edit page for a specific application.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}/edit")]
    public async Task<IActionResult> EditAsync([FromRoute] Guid id)
    {
        
        var query = new GetOpenIdApplicationByIdQuery(id);
        var application = await bus.InvokeAsync<OpenIdApplication?>(query);
        
        if (application is null)
            return NotFound();
        
        var model = application.Adapt<ApplicationEditModel>();
        
        return View(model);
    }

    /// <summary>
    /// Update the application with the given id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit")]
    public async Task<IActionResult> EditPostAsync([FromRoute] Guid id, ApplicationEditModel model)
    {
        if (!ModelState.IsValid){
            return View("Edit", model);
        }
        
        var command = model.Adapt<UpdateOpenIdApplicationCommand>();
        var result = await bus.InvokeAsync<Result>(command);
        if (result.IsError())
        {
            ModelState.AddModelError("", result.ErrorValue.Message);
            return View("Edit", model);
        }
        return RedirectToAction("Index");
    }

    /// <summary>
    /// HTMX endpoint to add a new URI Item to the post logout uri list.
    /// </summary>
    /// <param name="id">application id</param>
    /// <param name="postLogoutUri">uri to include into generated html</param>
    /// <param name="type">Type of the uri (Application.PostLogoutRedirectUris/)</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit/post-logout-uri")]
    public IActionResult AddPostLogout([FromRoute] Guid id,[FromForm] string postLogoutUri, [FromQuery] string type)
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
        return View("Edit/DynamicListItem", new DynamicListItem { Value = parsedUri.ToString(), Id = id , Name = "PostLogoutRedirectUris"});
    }

    /// <summary>
    /// HTMX endpoint to add a new URI Item to the redirect uri list.
    /// </summary>
    /// <param name="id">application id</param>
    /// <param name="redirectUri">uri to include into generated html</param>
    /// <param name="type">Type of the uri (Application.PostLogoutRedirectUris/)</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit/redirect-uri")]
    public IActionResult AddRedirectUri([FromRoute] Guid id,[FromForm] string redirectUri, [FromQuery] string type)
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
        return View("Edit/DynamicListItem", new DynamicListItem { Value = parsedUri.ToString(), Id = id , Name = "RedirectUris"});
    }
    
    /// <summary>
    /// HTMX endpoint to add a new permission item to the list.
    /// </summary>
    /// <param name="id">application id</param>
    /// <param name="permission">permission to include into generated html</param>
    /// <returns></returns>
    [HttpPost("{id:guid}/edit/permission")]
    public IActionResult AddPermission([FromRoute] Guid id,[FromForm] string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
        {
            return BadRequest("permission cannot be empty");
        }

        // TODO: Validate the permission format

        // Return the partial view for HTMX to add to the DOM
        return View("Edit/DynamicListItem", new DynamicListItem { Value = permission, Id = id, Name = "Permissions" });
    }


    
}