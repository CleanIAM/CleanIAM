using System.ComponentModel.DataAnnotations;
using Mapster;
using ManagementPortal.Application.Commands;
using ManagementPortal.Application.Queries.OpenIdClients;
using ManagementPortal.Core;
using ManagementPortal.Core.OpenIdApplication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Api.Views.Clients;

public class EditModel : PageModel
{
    private readonly IMessageBus _bus;

    public EditModel(IMessageBus bus)
    {
        _bus = bus;
    }

    [BindProperty]
    public OpenIdApplication Client { get; set; }

    [BindProperty]
    public string? RedirectUrisString { get; set; }
    
    [BindProperty]
    public string? PostLogoutRedirectUrisString { get; set; }
    
    [BindProperty]
    public string? PermissionsString { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Valid Client ID is required");
        }

        var query = new GetOpenIdApplicationByIdQuery(id);
        var application = await _bus.InvokeAsync<OpenIdApplication?>(query);
        
        if (application is null)
        {
            return NotFound();
        }
        
        Client = application;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Process redirect URIs from strings to Uri objects
            Client.RedirectUris = new HashSet<Uri>();
            if (!string.IsNullOrWhiteSpace(RedirectUrisString))
            {
                foreach (var uriString in RedirectUrisString.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (Uri.TryCreate(uriString.Trim(), UriKind.Absolute, out var uri))
                    {
                        Client.RedirectUris.Add(uri);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Invalid redirect URI: {uriString}");
                        return Page();
                    }
                }
            }

            // Process post-logout redirect URIs from strings to Uri objects
            Client.PostLogoutRedirectUris = new HashSet<Uri>();
            if (!string.IsNullOrWhiteSpace(PostLogoutRedirectUrisString))
            {
                foreach (var uriString in PostLogoutRedirectUrisString.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (Uri.TryCreate(uriString.Trim(), UriKind.Absolute, out var uri))
                    {
                        Client.PostLogoutRedirectUris.Add(uri);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Invalid post-logout redirect URI: {uriString}");
                        return Page();
                    }
                }
            }
            
            // Process permissions from string to HashSet<string>
            Client.Permissions = new HashSet<string>(StringComparer.Ordinal);
            if (!string.IsNullOrWhiteSpace(PermissionsString))
            {
                foreach (var permission in PermissionsString.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    Client.Permissions.Add(permission.Trim());
                }
            }
            
            // Update the application using the command
            var command = new UpdateOpenIdApplicationCommand(Client);
            var result = await _bus.InvokeAsync<Result>(command);

            if (result.IsError())
            {
                ModelState.AddModelError(string.Empty, result.ErrorValue.Message);
                return Page();
            }

            StatusMessage = "Client updated successfully";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return Page();
        }
    }
}